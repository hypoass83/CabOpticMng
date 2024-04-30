using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FatSod.Supply.Entities
{
    public class GenericComparer<T> : IEqualityComparer<T>
    {
        private PropertyInfo _PropertyInfo1;
        private PropertyInfo _PropertyInfo2;


        public GenericComparer()
        {
            
        }

        /// <summary>
        /// Cette méthode permet de changer la valeur d'un champ d'un objet ne sachant que le nom de ce champ.
        /// </summary>
        /// <typeparam name="T">Type de l'objet</typeparam>
        /// <param name="obj">Objet donc on souhaite changer la valeur de l'une de ses propriétés à partir de son nom</param>
        /// <param name="propertyName">La propriété donc on souhaite changer sa valeur</param>
        /// <param name="value">Nouvelle valeur à affecter au champ de l'objet</param>
        public static void SetValue/*<T>*/(T obj, string propertyName, object value)
        {
            // these should be cached if possible
            Type type = typeof(T);
            //Récupération de la propriété dans le type (de l'objet passé en paramètre)
            PropertyInfo _PropertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            
            if (_PropertyInfo == null)
            {
                throw new ArgumentException(string.Format("{0} is not a property of type {1}.", propertyName, typeof(T)));
            }

            //Modification éffective de la valeur du champ de l'objet
            _PropertyInfo.SetValue(obj, Convert.ChangeType(value, _PropertyInfo.PropertyType), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName1">The name of the property on type T 
        /// to perform the comparison on.</param>
        /// <param name="propertyName2">The name of the property on type T 
        /// to perform the comparison on.</param>
        public GenericComparer(string propertyName1, string propertyName2)
        {
            //store a reference to the property info object for use during the comparison
            _PropertyInfo1 = typeof(T).GetProperty(propertyName1, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            _PropertyInfo2 = typeof(T).GetProperty(propertyName2, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (_PropertyInfo1 == null || _PropertyInfo2 == null)
            {
                throw new ArgumentException(string.Format("{0} or {1} is not a property of type {2}.", propertyName1, propertyName2, typeof(T)));
            }
        }

        /// <summary>
        /// Creates a new instance of GenericComparer.
        /// </summary>
        /// <param name="propertyName">The name of the property on type T 
        /// to perform the comparison on.</param>
        public GenericComparer(string propertyName)
        {
            //store a reference to the property info object for use during the comparison
            _PropertyInfo1 = typeof(T).GetProperty(propertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public);
            if (_PropertyInfo1 == null)
            {
                throw new ArgumentException(string.Format("{0} is not a property of type {1}.", propertyName, typeof(T)));
            }
        }

       
        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            //get the current value of the comparison property of x and of y
            object xValue1 = _PropertyInfo1.GetValue(x, null);
            object yValue1 = _PropertyInfo1.GetValue(y, null);

            object xValue2 = (_PropertyInfo2 != null) ? _PropertyInfo2.GetValue(x, null) : null;
            object yValue2 = (_PropertyInfo2 != null) ? _PropertyInfo2.GetValue(x, null) : null;

            //if the xValue is null then we consider them equal if and only if yValue is null
            if (xValue1 == null && xValue2 == null)
                return (_PropertyInfo2 == null) ? yValue1 == null : (yValue1 == null && yValue2 == null);

            //use the default comparer for whatever type the comparison property is.
            return (_PropertyInfo2 == null) ? xValue1.Equals(yValue1) : (xValue1.Equals(yValue1) && xValue2.Equals(yValue2));
        }

        public int GetHashCode(T obj)
        {
            //get the value of the comparison property out of obj
            object propertyValue1 = _PropertyInfo1.GetValue(obj, null);
            object propertyValue2 = (_PropertyInfo2 != null) ?  _PropertyInfo2.GetValue(obj, null) : null;

            if (propertyValue1 == null && propertyValue2 == null)
                return 0;

            else
                return (_PropertyInfo2 == null) ? propertyValue1.GetHashCode() : (propertyValue1.GetHashCode() + propertyValue2.GetHashCode());
        }

        #endregion
    }

    


}
