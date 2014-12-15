﻿/*
 * Created by SharpDevelop.
 * User: wij
 * Date: 7/12/2014
 * Time: 6:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Security.Cryptography;
using System.Text;
using LogicNP.CryptoLicensing;

namespace EAAddinFramework.Licensing
{
	/// <summary>
	/// Description of License.
	/// </summary>
	public class License
	{
		private string _key;
		public bool floating {get;set;}
		public string client {get;set;}
		public bool isValid {get;private set;}
		public string key 
		{
			get
			{
				return this._key;
			}
			internal set
			{
				this._key = value;
			}
		}
		/// <summary>
		/// create a new License for the given license key and public key.
		/// </summary>
		/// <param name="key">the key</param>
		/// <param name="publicKey">the public key</param>
		public License(string key, string publicKey)
		{
			//for some strange reason EA wraps the key in a "{" and "}" so we need to remove them first
			key = key.Replace("{",string.Empty).Replace("}",string.Empty);
			this.key = key;
			//create the wrapped license
			CryptoLicense wrappedlicense = new CryptoLicense(key, publicKey);
			bool validateFloating = false;
			
			//if the license is not valid then it might be a floating license.
			//in that case we strip the first part until the "-" and try again.
			int startActualKey = key.IndexOf("-") +1;
			if (wrappedlicense.Status != LicenseStatus.Valid && startActualKey > 0)
			{
				key = key.Substring(startActualKey);
				wrappedlicense = new CryptoLicense(key, publicKey);
				validateFloating = true;
			}
			//get user data
			if (wrappedlicense.Status == LicenseStatus.Valid)
			{
				this.client = wrappedlicense.GetUserDataFieldValue("Client", "|");
				string isFloating = wrappedlicense.GetUserDataFieldValue("Isfloating","|");
				bool isFloatingBool;
				if(bool.TryParse(isFloating,out isFloatingBool))
				{
					this.floating = isFloatingBool;
				}
				//set validation status;
				if (validateFloating == this.floating)
				{
					this.isValid = true;
				}
			}
			
		}
		
	}
}