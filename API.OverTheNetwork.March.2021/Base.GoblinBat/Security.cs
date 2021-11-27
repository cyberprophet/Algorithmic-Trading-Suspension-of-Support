using ShareInvest.Catalog.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ShareInvest.Crypto
{
	public static class Security
	{
		public static string ConvertCrypto(bool type, byte[] key, string input)
		{
			var des = new DESCryptoServiceProvider()
			{
				Key = key,
				IV = key
			};
			var ms = new MemoryStream();
			var property = new
			{
				transform = type ? des.CreateEncryptor() : des.CreateDecryptor(),
				data = type ? Encoding.UTF8.GetBytes(input.ToCharArray()) : System.Convert.FromBase64String(input)
			};
			var cryStream = new CryptoStream(ms, property.transform, CryptoStreamMode.Write);
			var data = property.data;
			cryStream.Write(data, 0, data.Length);
			cryStream.FlushFinalBlock();

			return type ? System.Convert.ToBase64String(ms.ToArray()) : Encoding.UTF8.GetString(ms.GetBuffer());
		}
		public static string Decipher(string key, string api, string param) => ConvertString(ConvertCrypto(false, Encoding.ASCII.GetBytes(key.Substring(api.Length, 8)), param));
		public static string Decipher(string param)
		{
			var now = DateTime.Now.ToString(Base.LongDateFormat);

			return ConvertString(ConvertCrypto(false, Encoding.ASCII.GetBytes(now), param));
		}
		public static string Decipher(string email, string account) => ConvertString(ConvertCrypto(email.Length == 0, Encoding.ASCII.GetBytes(email.Replace("@", string.Empty).Replace(".", string.Empty)[^8..]), account));
		public static string Encrypt(string email, StringBuilder account)
		{
			var encrypt = email.Replace("@", string.Empty).Replace(".", string.Empty);
			var check = encrypt.Length > 8;

			if (check)
				return ConvertCrypto(check, Encoding.ASCII.GetBytes(encrypt[^8..]), ConvertASCII(account.ToString()));

			return null;
		}
		public static Account Encrypt(Account param, bool check)
		{
			if (check && GetGrantAccess(param.Security.ToCharArray()))
			{
				var list = new List<string>();
				var now = DateTime.Now.ToString(Base.LongDateFormat);

				foreach (var str in param.Number)
					if (string.IsNullOrEmpty(str) is false && str.Length == 0xA && str[^2..].CompareTo("32") < 0)
						list.Add(ConvertCrypto(check, Encoding.ASCII.GetBytes(now), ConvertASCII(str)));

				return new Account
				{
					Length = list.Count,
					Number = list.ToArray(),
					Security = ConvertCrypto(check, Encoding.ASCII.GetBytes(now), ConvertASCII(param.Security)),
					Identity = Encrypt(param.Identity),
					Name = ConvertName(param.Name)
				};
			}
			return param;
		}
		public static string ConvertName(string param)
		{
			string name = param.Replace(corporation, string.Empty).Replace(joint, string.Empty).Trim(), confirm = name.Length > 2 ? (name.Length > 3 ? name.Remove(3) : name).Remove(1, 1).Insert(1, "*") : name;

			return confirm.Length == 3 ? confirm : (confirm.Length == 2 ? confirm.Insert(1, "*") : string.Concat(confirm, "*C"));
		}
		public static string Encrypt(Privacies key, string account, bool check)
		{
			if (check && (account.Length == 0xB || account.Length == 0x15))
				return ConvertCrypto(check, Encoding.ASCII.GetBytes(key.Security.Substring(key.SecuritiesAPI.Length, account.Length - (account.Length == 0xB ? 3 : 0xD))), ConvertASCII(account));

			return null;
		}
		public static string Encrypt(string param) => System.Convert.ToBase64String(new SHA512Managed().ComputeHash(Encoding.ASCII.GetBytes(param)));
		public static bool GetGrantAccess(char[] array)
		{
			int uCount = 0, dCount = 0, lCount = 0;

			foreach (char ch in array)
			{
				if (char.IsUpper(ch))
					uCount++;

				else if (char.IsDigit(ch))
					dCount++;

				else if (char.IsLower(ch))
					lCount++;

				else
					return array.Length == 0;
			}
			if (array.Length == uCount + dCount)
				return lCount == 0 && char.IsDigit(array[5]) && char.IsDigit(array[^6]);

			else
				return lCount == 0 && array.Length > 0;
		}
		public static string GetRoute(string param, string code) => string.Concat(GetRoute(param), "?key=", string.IsNullOrEmpty(code) ? code : HttpUtility.UrlEncode(code));
		public static string GetRoute(string param, string id, string account) => $"{GetRoute(param)}?id={HttpUtility.UrlEncode(id)}&account={HttpUtility.UrlEncode(account)}";
		public static string GetRoute(string param) => Path.Combine(route, param);
		public static string GetRoute(Type type) => Path.Combine(route, type.Name);
		public static string Connection => connection;
		static string ConvertString(string crypto)
		{
			var queue = new Queue<char>();
			var sb = new StringBuilder();

			for (int i = 0; i < crypto.Length - 3; i += 3)
				if (int.TryParse(crypto.Substring(i, 3), out int str))
					queue.Enqueue((char)str);

			while (queue.Count > 0)
				sb.Append(queue.Dequeue());

			return sb.ToString();
		}
		static string ConvertASCII(string param)
		{
			var sb = new StringBuilder();

			foreach (int str in param.ToCharArray())
				sb.Append(str.ToString("D3"));

			return sb.ToString();
		}
		const string corporation = "주식회사";
		const string joint = @"(주)";
		const string route = "Algorithmic";
		const string connection = "Data:CommandAPIConnection:ConnectionString";
	}
}