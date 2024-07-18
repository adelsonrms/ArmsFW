using ArmsFW.Lib.Web.HttpRest;
using ArmsFW.Lib.Web.Json;
using ArmsFW.Services.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static class JSON
{
	public static TObject JsonTextToObject<TObject>(string content)
	{
		try
		{
			return System.Text.Json.JsonSerializer.Deserialize<TObject>(content, (JsonSerializerOptions)null);
		}
		catch
		{
			try
			{
				return (TObject)Activator.CreateInstance(typeof(TObject));
			}
			catch (Exception)
			{
				return (TObject)(object)new { content };
			}
		}
	}

	public static TObject JsonToObject<TObject>(string content)
	{
		try
		{
			if (content.IsJson()) return JsonConvert.DeserializeObject<TObject>(content);
		}
		catch
		{
		}
		return default(TObject);
	}

	public static string ToJson<T>(this IList<T> list, bool? identado = true, bool? ignoraNull = true)
	{
		return JSON.ToJson<IList<T>>(list, identado, ignoraNull);
	}

	public static bool IsJson(this string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			return false;
		}

		return (json.Trim().StartsWith("{") && json.Trim().EndsWith("}")) ||
			   (json.Trim().StartsWith("[") && json.Trim().EndsWith("]"));
	}

	public static string ToJson<T>(this IEnumerable<T> list)
	{
		return ObjectToJson(list);
	}

	public static string ToJson<T>(this ICollection<T> list)
	{
		return ObjectToJson(list);
	}

	public static string ToJson(this JsonData jsonObject)
	{
		return ObjectToJson(jsonObject);
	}

	public static string ToJson(this object jsonObject, bool? identado = true)
	{
		return ToJson<object>(jsonObject, identado, true);
	}

	private static string ObjectToJson(dynamic jsonObject)
	{
		return JSON.ToJson<object>(jsonObject);
	}

	/// <summary>
	/// Converte um objeto de um tipo generico para string Json
	/// </summary>
	/// <typeparam name="T">Instancia do objeto</typeparam>
	/// <param name="jsonObject"></param>
	/// <returns></returns>
	public static string ToJson<T>(T jsonObject, bool? identado, bool? ignoraNull)
	{
		try
		{
			Formatting formato = Formatting.None;
			NullValueHandling trataNull = NullValueHandling.Include;

			if (identado.HasValue && identado.GetValueOrDefault()) formato = Formatting.Indented;
			if (ignoraNull.HasValue && ignoraNull.GetValueOrDefault()) trataNull = NullValueHandling.Ignore;

			return Regex.Unescape(JsonConvert.SerializeObject(jsonObject, formato, new JsonSerializerSettings { NullValueHandling = trataNull }));
		}
		catch (Exception ex)
		{
			LogServices.Debug($"Erro na serialização JSON do objeto '{typeof(T).Name}' : {ex.Source}-{ex.Message}");
			return "{erro:'Falha na conversao do objeto " + typeof(T).Name + " para json. Detalhe : " + ex.Source + " - " + ex.Message + "'}";
		}
	}

	public static async Task<TObject> LoadJSonFileToObject<TObject>(string jsonFilePath) where TObject : class
	{
		try
		{
			if (File.Exists(jsonFilePath))
			{
				return JsonConvert.DeserializeObject<TObject>(File.ReadAllText(jsonFilePath));
			}
		}
		catch (Exception ex)
		{
			return await Task.FromException<TObject>(new JsonFileLoadException<TObject>("Falha na leitura/Desserialização do conteudo Json para o objeto. \n" + ex.Message, ex, Activator.CreateInstance<TObject>())
			{
				JsonFIle = jsonFilePath,
				JsonException = ((ex is JsonSerializationException) ? ex : null)
			});
		}

		return default(TObject);

	}

	public static JsonResult<TObject> LoadFromFile<TObject>(string jsonFilePath) where TObject : class
	{
		JsonResult<TObject> jsonResult = new JsonResult<TObject>();
		try
		{
			if (File.Exists(jsonFilePath))
			{
				jsonResult.Result = JsonConvert.DeserializeObject<TObject>(File.ReadAllText(jsonFilePath));
				return jsonResult;
			}
			jsonResult.Result = Activator.CreateInstance<TObject>();
			return jsonResult;
		}
		catch (Exception ex)
		{
			jsonResult.Result = Activator.CreateInstance<TObject>();
			JsonFileLoadException<TObject> ex2 = new JsonFileLoadException<TObject>("Falha na leitura/Desserialização do conteudo Json para o objeto. \n" + ex.Message, ex, Activator.CreateInstance<TObject>());
			ex2.JsonFIle = jsonFilePath;
			ex2.JsonException = ((ex is JsonSerializationException) ? ex : null);
			jsonResult.Exception = ex2;
			return jsonResult;
		}
	}


	public static TObject Carregar<TObject>(this string json) where TObject : class
	{
		JsonResult<TObject> jsonResult = new JsonResult<TObject>();
		string strConteudo = json;

		try
		{
			//Se for um arquivo, carrega o conteudo
			if (File.Exists(json)) strConteudo = File.ReadAllText(json);

			if (IsJson(strConteudo))
            {
                strConteudo = strConteudo.Replace(@"\", @"\\");

				return JsonConvert.DeserializeObject<TObject>(strConteudo);
			}
		}
		catch
		{
		}
		return default(TObject);
	}

	public static bool Gravar(dynamic jsonObject, string localArquivo, bool acresentar = false)
	{
		try
		{
			string stringObjeto = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

			if (!(File.Exists(localArquivo) && acresentar))
			{
				File.AppendAllTextAsync(localArquivo, stringObjeto);

			}
			else
			{
				//Gravar um novo
				File.WriteAllTextAsync(localArquivo, stringObjeto);
			}

			return true;
		}
		catch
		{
		}

		return false;
	}


	public static async Task<TObject> SaveJSonObjectToFile<TObject>(dynamic jsonObject, string jsonFilePath, bool appendIfExists = true) where TObject : class
	{
		dynamic val = JsonConvert.SerializeObject(jsonObject, (Formatting)1);
		if (!(File.Exists(jsonFilePath) && appendIfExists))
		{
			File.WriteAllText(jsonFilePath, val);
		}
		else
		{
			File.AppendAllText(jsonFilePath, val);
		}
		return await LoadJSonFileToObject<TObject>(jsonFilePath);
	}

	public static async Task SaveJSonObjectToFile(dynamic jsonObject, string jsonFilePath)
	{
		try
		{
			await Task.Run(delegate
			{
				object obj2 = JsonConvert.SerializeObject(jsonObject, (Formatting)1);
				if (File.Exists(jsonFilePath))
				{
					File.AppendAllText(jsonFilePath, (dynamic)obj2);
				}
				else
				{
					File.WriteAllText(jsonFilePath, (dynamic)obj2);
				}
			});
		}
		catch
		{
		}
	}
}