#region license
// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net
#endregion

namespace PetStoreWeb.Components
{
	using System;

	[Serializable()]
	public class PurchaseOrder
	{
		public readonly long id;	
		public readonly Account account;
		public readonly ContactInfo shippingInfo;
		public readonly ContactInfo billingInfo;
		public readonly CartItem[] items;
		public readonly DateTime date;
		
		public PurchaseOrder(long id, Account account, 
			ContactInfo shippingInfo, ContactInfo billingInfo, 
			CartItem[] items, DateTime date) 
		{
			this.id = id;
			this.account = account;
			this.shippingInfo = shippingInfo;
			this.billingInfo = billingInfo;
			this.items = items;
			this.date = date;
		}
		
		public static bool checkCreate(long id, Account account, 
			ContactInfo shippingInfo, ContactInfo billingInfo, 
			CartItem[] items, DateTime date) 
		{
			if (account == null) return false;
			if (shippingInfo == null) return false;
			if (billingInfo == null) return false;
			if (items == null) return false;
			return true;
		}
			
		public double total() 
		{
			double total = 0d;
			
			for (int i = 0; i < items.Length; i++) 
			{
				total += items[i].price * items[i].getQuantity();				
			}
			
			return total;	
		}
	}
}
