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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.XPath;
using Bamboo.Prevalence.Implementation;
using NUnit.Framework;
using Bamboo.Prevalence.XPath;

namespace Bamboo.Prevalence.XPath.Tests
{
	public class Address
	{
		string _street;

		short _number;

		public Address(string street, short number)
		{
			_street = street;
			_number = number;
		}

		public string Street
		{
			get
			{
				return _street;
			}
		}

		public short Number
		{
			get
			{
				return _number;
			}
		}
	}

	public class Product
	{
		string _name;

		StringCollection _categories;

		public Product(string name)
		{
			_name = name;
			_categories = new StringCollection();
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public StringCollection Categories
		{
			get
			{
				return _categories;
			}
		}
	}

	public class OrderItem
	{
		Product _product;

		int _quantity;

		public OrderItem(Product product, int quantity)
		{
			_product = product;
			_quantity = quantity;
		}

		public Product Product
		{
			get
			{
				return _product;
			}
		}

		public int Quantity
		{
			get
			{
				return _quantity;
			}
		}
	}

	public class Order
	{
		Customer _customer;

		ArrayList _items;

		public Order(Customer customer)
		{
			_customer = customer;
			_items = new ArrayList();
		}

		public Customer Customer
		{
			get
			{
				return _customer;
			}
		}

		public OrderItem[] Items
		{
			get
			{
				return (OrderItem[])_items.ToArray(typeof(OrderItem));
			}
		}

		public void Add(OrderItem item)
		{
			_items.Add(item);
		}
	}

	public class Customer
	{
		string _fname;

		string _lname;

		Address _address;

		IDictionary _properties;

		public string Email;

		public Customer(string fname, string lname, Address address)
		{
			_fname = fname;
			_lname = lname;
			_address = address;
			_properties = new Hashtable();
		}

		public IDictionary Properties
		{
			get
			{
				return _properties;
			}
		}

		public Address Address
		{
			get
			{
				return _address;
			}
		}

		public string FirstName
		{
			get
			{
				return _fname;
			}
		}

		public string LastName
		{
			get
			{
				return _lname;
			}
		}
	}

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class XPathObjectNavigatorTest : Assertion
	{
		[Test]
		public void TestSimpleProperties()
		{
			Address address = new Address("Al. Calder�o Branco", 784);
			Customer customer = new Customer("Rodrigo", "Oliveira", address);

			XPathObjectNavigator context = new XPathObjectNavigator(customer);
			XPathNodeIterator i = context.Select("/Customer/Address/Street");			
			AssertEquals(1, i.Count);
			AssertEquals(true, i.MoveNext());			
			AssertEquals(customer.Address.Street, i.Current.Value);
			AssertEquals(customer.Address.Street, ((XPathObjectNavigator)i.Current).Node);

			i = context.Select("FirstName");
			AssertEquals(1, i.Count);
			AssertEquals(true, i.MoveNext());
			AssertEquals(customer.FirstName, i.Current.Value);

			i = context.Select("/Customer/LastName");						
			AssertEquals(true, i.MoveNext());
			AssertEquals(customer.LastName, i.Current.Value);
		}

		[Test]
		public void TestIListProperties()
		{
			Product p1 = new Product("egg");
			Product p2 = new Product("Monty Python Flying Circus Box");
			p2.Categories.Add("Silly Stuff");

			Customer c1 = new Customer("Rodrigo", "Oliveira", new Address("Al. Ribeir�o Preto", 487));
			Customer c2 = new Customer("Marcia", "Longo", new Address("Al. Ribeir�o Preto", 487));

			Order o1 = new Order(c1);
			o1.Add(new OrderItem(p1, 10));
			o1.Add(new OrderItem(p2, 1));

			Order o2 = new Order(c2);
			o2.Add(new OrderItem(p1, 15));
			o2.Add(new OrderItem(p2, 1));

			Order[] orders = new Order[] { o1, o2 };
			XPathObjectNavigator navigator = new XPathObjectNavigator(orders, "Orders");			
			AssertEquals(2, navigator.Select("//Order").Count);
			AssertEquals(2, navigator.Select("Order").Count);
			AssertEquals(o1, navigator.SelectObject("Order[1]"));
			AssertEquals(o2, navigator.SelectObject("Order[2]"));

			AssertEquals(o1, navigator.SelectObject("//Order[Customer/FirstName='Rodrigo']"));
			AssertEquals(o2, navigator.SelectObject("//Order[Customer/LastName='Longo']"));

			XPathNodeIterator i = navigator.Select("//Product[Name='egg']");
			AssertEquals(2, i.Count);
			AssertEquals(true, i.MoveNext());
			AssertEquals(p1, ((XPathObjectNavigator)i.Current).Node);

			AssertEquals(o2.Items[0], navigator.SelectObject("//OrderItem[Quantity>10]"));
			AssertEquals(p2, navigator.SelectObject("//Product[Categories/String='Silly Stuff']"));
		}

		[Test]
		public void TestIDictionaryProperties()
		{
			Customer customer = new Customer("Rodrigo", "Oliveira", new Address("Penny Lane", 64));
			customer.Properties["email"] = "rodrigobamboo@users.sourceforge.net";

			XPathObjectNavigator navigator = new XPathObjectNavigator(customer);
			AssertEquals(customer.Properties["email"], navigator.SelectObject("Properties/email"));
		}

		[Test]
		public void TestSelectObjects()
		{
			Address address = new Address("Strawberry Street", 45);
			Customer customer1 = new Customer("Rodrigo", "Oliveira", address);
			Customer customer2 = new Customer("Marcia", "Longo", address);

			Customer[] customers = { customer1, customer2 };
			XPathObjectNavigator navigator = new XPathObjectNavigator(customers);
			object[] actual = navigator.SelectObjects("Customer[Address/Number = 45]");
			AssertEquals(2, actual.Length);
			AssertEquals(customer1, actual[0]);
			AssertEquals(customer2, actual[1]);
		}

		[Test]
		public void TestSelectByField()
		{
			Customer customer1 = new Customer("Rodrigo", "Oliveira", new Address("al. Calder�o Branco", 45));
			customer1.Email = "rbo@acm.org";

			XPathObjectNavigator navigator = new XPathObjectNavigator(customer1);
			AssertSame(customer1.Email, navigator.SelectObject("Email"));

			AssertSame(customer1, navigator.SelectObject("/Customer[Email='rbo@acm.org']"));
		}
	}
}
