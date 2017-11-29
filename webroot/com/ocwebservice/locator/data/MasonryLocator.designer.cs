﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace com.ocwebservice.locator.data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Stone")]
	public partial class MasonryLocatorDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public MasonryLocatorDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["OwensCorning.Locator.Stone"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public MasonryLocatorDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MasonryLocatorDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MasonryLocatorDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MasonryLocatorDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<com.ocwebservice.locator.model.RawMasonryLocatorResult> RawMasonryLocatorResults
		{
			get
			{
				return this.GetTable<com.ocwebservice.locator.model.RawMasonryLocatorResult>();
			}
		}
		
		public System.Data.Linq.Table<com.ocwebservice.locator.model.RawMasonryLocation> RawMasonryLocations
		{
			get
			{
				return this.GetTable<com.ocwebservice.locator.model.RawMasonryLocation>();
			}
		}
	}
}
namespace com.ocwebservice.locator.model
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PD_CSDealer")]
	public partial class RawMasonryLocatorResult
	{
		
		private string _Company;
		
		private string _ShipAddress1;
		
		private string _ShipCity;
		
		private string _ShipState;
		
		private string _ShipZip;
		
		private string _Phone;
		
		private string _URL;
		
		private string _Email;
		
		private System.Nullable<char> _WebVisible;
		
		private int _ProDesk_ID;
		
		private string _Member_Type_Code;
		
		private System.DateTime _DataUploadDateTime;
		
		private char _Showroom;
		
		private string _Member_Level;
		
		private char _ProStone;
		
		private System.Nullable<System.DateTime> _Date_Enrolled;
		
		private string _Product_Lead_Type_Code;
		
		private string _Product_Lead_Type_Description;
		
		private string _RegionId;
		
		private string _RegionName;
		
		private string _SalesRepID;
		
		private string _SalesRepFirstName;
		
		private string _SalesRepLastName;
		
		private string _SalesRepEmailID;
		
		private string _SalesRepUserName;
		
		public RawMasonryLocatorResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Company", DbType="VarChar(60)")]
		public string Company
		{
			get
			{
				return this._Company;
			}
			set
			{
				if ((this._Company != value))
				{
					this._Company = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ShipAddress1", DbType="VarChar(128)")]
		public string ShipAddress1
		{
			get
			{
				return this._ShipAddress1;
			}
			set
			{
				if ((this._ShipAddress1 != value))
				{
					this._ShipAddress1 = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ShipCity", DbType="VarChar(80)")]
		public string ShipCity
		{
			get
			{
				return this._ShipCity;
			}
			set
			{
				if ((this._ShipCity != value))
				{
					this._ShipCity = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ShipState", DbType="VarChar(50)")]
		public string ShipState
		{
			get
			{
				return this._ShipState;
			}
			set
			{
				if ((this._ShipState != value))
				{
					this._ShipState = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ShipZip", DbType="VarChar(13)")]
		public string ShipZip
		{
			get
			{
				return this._ShipZip;
			}
			set
			{
				if ((this._ShipZip != value))
				{
					this._ShipZip = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Phone", DbType="VarChar(20)")]
		public string Phone
		{
			get
			{
				return this._Phone;
			}
			set
			{
				if ((this._Phone != value))
				{
					this._Phone = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_URL", DbType="VarChar(100)")]
		public string URL
		{
			get
			{
				return this._URL;
			}
			set
			{
				if ((this._URL != value))
				{
					this._URL = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Email", DbType="VarChar(60)")]
		public string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				if ((this._Email != value))
				{
					this._Email = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_WebVisible", DbType="VarChar(1)")]
		public System.Nullable<char> WebVisible
		{
			get
			{
				return this._WebVisible;
			}
			set
			{
				if ((this._WebVisible != value))
				{
					this._WebVisible = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProDesk_ID", DbType="Int NOT NULL")]
		public int ProDesk_ID
		{
			get
			{
				return this._ProDesk_ID;
			}
			set
			{
				if ((this._ProDesk_ID != value))
				{
					this._ProDesk_ID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Member_Type_Code", DbType="VarChar(5)")]
		public string Member_Type_Code
		{
			get
			{
				return this._Member_Type_Code;
			}
			set
			{
				if ((this._Member_Type_Code != value))
				{
					this._Member_Type_Code = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DataUploadDateTime", DbType="DateTime NOT NULL")]
		public System.DateTime DataUploadDateTime
		{
			get
			{
				return this._DataUploadDateTime;
			}
			set
			{
				if ((this._DataUploadDateTime != value))
				{
					this._DataUploadDateTime = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Showroom", DbType="VarChar(1) NOT NULL")]
		public char Showroom
		{
			get
			{
				return this._Showroom;
			}
			set
			{
				if ((this._Showroom != value))
				{
					this._Showroom = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Member_Level", DbType="VarChar(10)")]
		public string Member_Level
		{
			get
			{
				return this._Member_Level;
			}
			set
			{
				if ((this._Member_Level != value))
				{
					this._Member_Level = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProStone", DbType="VarChar(1) NOT NULL")]
		public char ProStone
		{
			get
			{
				return this._ProStone;
			}
			set
			{
				if ((this._ProStone != value))
				{
					this._ProStone = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Date_Enrolled", DbType="DateTime")]
		public System.Nullable<System.DateTime> Date_Enrolled
		{
			get
			{
				return this._Date_Enrolled;
			}
			set
			{
				if ((this._Date_Enrolled != value))
				{
					this._Date_Enrolled = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Product_Lead_Type_Code", DbType="VarChar(15)")]
		public string Product_Lead_Type_Code
		{
			get
			{
				return this._Product_Lead_Type_Code;
			}
			set
			{
				if ((this._Product_Lead_Type_Code != value))
				{
					this._Product_Lead_Type_Code = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Product_Lead_Type_Description", DbType="VarChar(75)")]
		public string Product_Lead_Type_Description
		{
			get
			{
				return this._Product_Lead_Type_Description;
			}
			set
			{
				if ((this._Product_Lead_Type_Description != value))
				{
					this._Product_Lead_Type_Description = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RegionId", DbType="VarChar(8)")]
		public string RegionId
		{
			get
			{
				return this._RegionId;
			}
			set
			{
				if ((this._RegionId != value))
				{
					this._RegionId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RegionName", DbType="VarChar(50)")]
		public string RegionName
		{
			get
			{
				return this._RegionName;
			}
			set
			{
				if ((this._RegionName != value))
				{
					this._RegionName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalesRepID", DbType="VarChar(10)")]
		public string SalesRepID
		{
			get
			{
				return this._SalesRepID;
			}
			set
			{
				if ((this._SalesRepID != value))
				{
					this._SalesRepID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalesRepFirstName", DbType="VarChar(32)")]
		public string SalesRepFirstName
		{
			get
			{
				return this._SalesRepFirstName;
			}
			set
			{
				if ((this._SalesRepFirstName != value))
				{
					this._SalesRepFirstName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalesRepLastName", DbType="VarChar(32)")]
		public string SalesRepLastName
		{
			get
			{
				return this._SalesRepLastName;
			}
			set
			{
				if ((this._SalesRepLastName != value))
				{
					this._SalesRepLastName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalesRepEmailID", DbType="VarChar(100)")]
		public string SalesRepEmailID
		{
			get
			{
				return this._SalesRepEmailID;
			}
			set
			{
				if ((this._SalesRepEmailID != value))
				{
					this._SalesRepEmailID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SalesRepUserName", DbType="VarChar(32)")]
		public string SalesRepUserName
		{
			get
			{
				return this._SalesRepUserName;
			}
			set
			{
				if ((this._SalesRepUserName != value))
				{
					this._SalesRepUserName = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.RfgZips")]
	public partial class RawMasonryLocation
	{
		
		private string _PostalCode;
		
		private string _DistCodeListID;
		
		private string _TradeArea;
		
		private string _TradeCodeListID;
		
		private string _State;
		
		private string _County;
		
		private string _City;
		
		private System.Nullable<double> _Latitude;
		
		private System.Nullable<double> _Longitude;
		
		public RawMasonryLocation()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="Zip", Storage="_PostalCode", DbType="NVarChar(10) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string PostalCode
		{
			get
			{
				return this._PostalCode;
			}
			set
			{
				if ((this._PostalCode != value))
				{
					this._PostalCode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DistCodeListID", DbType="NVarChar(5) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string DistCodeListID
		{
			get
			{
				return this._DistCodeListID;
			}
			set
			{
				if ((this._DistCodeListID != value))
				{
					this._DistCodeListID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TradeArea", DbType="NVarChar(50)", UpdateCheck=UpdateCheck.Never)]
		public string TradeArea
		{
			get
			{
				return this._TradeArea;
			}
			set
			{
				if ((this._TradeArea != value))
				{
					this._TradeArea = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TradeCodeListID", DbType="NVarChar(5) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string TradeCodeListID
		{
			get
			{
				return this._TradeCodeListID;
			}
			set
			{
				if ((this._TradeCodeListID != value))
				{
					this._TradeCodeListID = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="St", Storage="_State", DbType="NVarChar(3)", UpdateCheck=UpdateCheck.Never)]
		public string State
		{
			get
			{
				return this._State;
			}
			set
			{
				if ((this._State != value))
				{
					this._State = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_County", DbType="NVarChar(50)", UpdateCheck=UpdateCheck.Never)]
		public string County
		{
			get
			{
				return this._County;
			}
			set
			{
				if ((this._County != value))
				{
					this._County = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_City", DbType="NVarChar(50)", UpdateCheck=UpdateCheck.Never)]
		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				if ((this._City != value))
				{
					this._City = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="Lat", Storage="_Latitude", DbType="Float", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<double> Latitude
		{
			get
			{
				return this._Latitude;
			}
			set
			{
				if ((this._Latitude != value))
				{
					this._Latitude = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="Long", Storage="_Longitude", DbType="Float", UpdateCheck=UpdateCheck.Never)]
		public System.Nullable<double> Longitude
		{
			get
			{
				return this._Longitude;
			}
			set
			{
				if ((this._Longitude != value))
				{
					this._Longitude = value;
				}
			}
		}
	}
}
#pragma warning restore 1591