using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFileRenamer
{
	public abstract class ViewModel : ObservableObject, IDataErrorInfo
	{
		#region Implementation of IDataErrorInfo

		/// <summary>
		/// Gets the error message for the property with the given name.
		/// </summary>
		/// <returns>
		/// The error message for the property. The default is an empty string ("").
		/// </returns>
		/// <param name="columnName">The name of the property whose error message to get. </param>
		public string this[string columnName]
		{
			get { return OnValidate(columnName); }
		}

		/// <summary>
		/// Gets an error message indicating what is wrong with this object.
		/// </summary>
		/// <returns>
		/// An error message indicating what is wrong with this object. The default is an empty string ("").
		/// </returns>
		public string Error {
			get
			{
				throw new NotSupportedException();
			} }

		#endregion

		protected virtual string OnValidate(string propertyName)
		{
			var context = new ValidationContext(this) {MemberName = propertyName};
			var results = new Collection<ValidationResult>();
			var isValid = Validator.TryValidateObject(this, context, results, true);

			return !isValid ? results[0].ErrorMessage : null;
		}


	}
}
