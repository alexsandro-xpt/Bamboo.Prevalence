#region License
// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2002 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net
#endregion

using System;
using System.Collections;
using System.Reflection;

namespace Bamboo.Prevalence.Collections
{
	/// <summary>
	/// Compares objects by property (the property value
	/// must be IComparable).
	/// </summary>
	public class ObjectPropertyComparer : IComparer
	{
		PropertyInfo _property;

		public ObjectPropertyComparer(Type objectType, string propertyName)
		{
			MemberInfo[] members = objectType.GetMember(propertyName, MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public);
			if (1 != members.Length)
			{
				throw new ArgumentException(string.Format("Could not resolve the property name \"{0}\"!", propertyName), propertyName);
			}
			_property = (PropertyInfo)members[0];
		}

		public ObjectPropertyComparer(PropertyInfo property)
		{
			if (null == property)
			{
				throw new ArgumentNullException("property");
			}
			_property = property;
		}
		
		public int Compare(object lhs, object rhs)
		{
			object lhsValue = _property.GetValue(lhs, null);
			object rhsValue = _property.GetValue(rhs, null);

			int value = 0;
			if (null != lhsValue)
			{
				value = ((IComparable)lhsValue).CompareTo(rhsValue);
			}
			else if (null != rhsValue)
			{
				value = ((IComparable)rhsValue).CompareTo(lhsValue);
				value *= -1; // inverts the value since we changed the comparison
			}
			return value;
		}
	}
}
