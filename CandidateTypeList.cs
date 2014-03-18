namespace Constellation.Sitecore.Presentation
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Creates a list of Types that descend from the Required Ancestor Type.
	/// The Type list is limited to non-abstract classes.
	/// </summary>
	public class CandidateTypeList
	{
		#region Fields
		/// <summary>
		/// The internal list of candidate types. Do not reference this list directly.
		/// </summary>
		private readonly List<Type> candidateTypes = new List<Type>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CandidateTypeList"/> class.
		/// </summary>
		/// <param name="requiredAncestorType">The Type that all members of the list must inherit.</param>
		public CandidateTypeList(Type requiredAncestorType)
		{
			this.RequiredAncestorType = requiredAncestorType;
			this.CreateCandidateList();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets an initialized list of types.
		/// </summary>
		public IEnumerable<Type> CandidateTypes
		{
			get
			{
				return this.candidateTypes;
			}
		}

		/// <summary>
		/// Gets the Type that all members of the list must inherit.
		/// </summary>
		public Type RequiredAncestorType { get; private set; }
		#endregion

		/// <summary>
		/// Gets types that can actually be loaded by reflection. Handles the case where a
		/// type prerequisite isn't in the currently running application.
		/// </summary>
		/// <remarks>
		/// See http://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx for details.
		/// </remarks>
		/// <param name="assembly">The assembly to inspect.</param>
		/// <returns>A list of Types that can be loaded.</returns>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Phil Haack is his name.")]
		private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}

			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

		/// <summary>
		/// Determines if the candidate Type descends from the supplied ancestor Type.
		/// </summary>
		/// <param name="type">The Type to test.</param>
		/// <param name="ancestor">The Type that must be inherited.</param>
		/// <returns>True if the candidate Type inherits from the ancestor Type.</returns>
		private static bool DescendsFromAncestorType(Type type, Type ancestor)
		{
			if (type == null || type == typeof(object))
			{
				return false; // walked all the way up the inheritance tree with no match.
			}

			if (type.IsAssignableFrom(ancestor))
			{
				return true; // works for basic interface and class inheritance.
			}

			if (ancestor.IsGenericType && DescendsFromGeneric(type, ancestor))
			{
				return true;
			}

			return DescendsFromAncestorType(type.BaseType, ancestor);
		}

		/// <summary>
		/// Determines if the candidate Type implements the ancestor Generic.
		/// </summary>
		/// <param name="type">The Type to test.</param>
		/// <param name="ancestor">The Generic type that must be implemented.</param>
		/// <returns>True if the candidate Type implements the ancestor Type.</returns>
		private static bool DescendsFromGeneric(Type type, Type ancestor)
		{
			if (type == null || type == typeof(object))
			{
				return false; // We've reached the top of the inheritance stack.
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == ancestor)
			{
				return true;
			}

			var interfaceTypes = type.GetInterfaces();

			foreach (var i in interfaceTypes)
			{
				if (i.IsGenericType && i.GetGenericTypeDefinition() == ancestor)
				{
					return true;
				}
			}

			return DescendsFromAncestorType(type.BaseType, ancestor);
		}

		/// <summary>
		/// Creates a list of Types that descend from <see cref="RequiredAncestorType"/>
		/// based on the current running assemblies.
		/// </summary>
		private void CreateCandidateList()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies)
			{
				var types = GetLoadableTypes(assembly);

				foreach (var type in types)
				{
					if (type.IsClass && !type.IsAbstract && DescendsFromAncestorType(type, this.RequiredAncestorType))
					{
						this.candidateTypes.Add(type);
					}
				}
			}
		}
	}
}
