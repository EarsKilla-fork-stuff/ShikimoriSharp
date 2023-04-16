using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShikimoriSharp.Exceptions;

namespace ShikimoriSharp.Bases
{
    public abstract class ApiBase
    {
        private readonly ApiClient _apiClient;

        protected bool Requires(AccessToken token, IEnumerable<string> scopes)
        {
            var scope = token.Scope.Split(" ");
            if (scopes.All(it => scope.Any(x => x == it)))
                throw new NotInScopeException();
            return true;
        }
        protected ApiBase(Version version, ApiClient apiClient)
        {
            Version = version;
            _apiClient = apiClient;
        }

        public Version Version { get; }
        private string Site => $"https://shikimori.me/api/{GetThing()}";

        private string GetThing()
        {
            return Version switch
            {
                Version.v1 => "",
                _ => Version + "/"
            };
        }

        private static HttpContent DeserializeToRequest<T>(T obj, bool useJsonSerializer = false, string paramName = default)
        {
            var content = new MultipartFormDataContent();
            if (useJsonSerializer)
            {
                var strContent = new StringContent(JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                }));
                if (string.IsNullOrEmpty(paramName))
                    content.Add(strContent);
                else
                    content.Add(strContent, paramName);
            }
            else
            {
                if (obj is null) return null;
                var typeooft = obj.GetType();
                var type = typeooft.GetFields(BindingFlags.Public | BindingFlags.Instance);
                var typeEnum = type.Select(it => new
                    {
                        it.Name, Value = it switch
                        {
                            _ when IsEnum(it) => GetEnumValue(obj, it),
                            _ => it.GetValue(obj)
                        }
                    })
                    .Where(it => !(it.Value is null));
                foreach (var i in typeEnum)
                    content.Add(new StringContent(i.Value.ToString()), string.IsNullOrEmpty(paramName) ? i.Name : $"{paramName}[{i.Name}]");
            }

            return content;
        }

        private static bool IsEnum(FieldInfo fieldInfo)
            => fieldInfo switch
            {
                _ when fieldInfo.FieldType.IsEnum => true,
                _ when Nullable.GetUnderlyingType(fieldInfo.FieldType)?.IsEnum == true => true,
                _ => false
            };

        private static object GetEnumValue(object obj, FieldInfo fieldInfo)
        {
            var value = fieldInfo.GetValue(obj);
            if (value == null)
                return null;

            var fieldType = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType;

            try
            {
                var members = fieldType.GetMember(value.ToString());
                var member = members.FirstOrDefault(x => x.DeclaringType == fieldType);
                if (member == null)
                    return value;

                var memberAttribute = member.GetCustomAttribute<EnumMemberAttribute>(false);
                if (memberAttribute == null)
                    return value;

                return memberAttribute.Value;
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(Debugger.IsAttached, $"Cannot get value from enum {fieldInfo.FieldType}\n{e}");
                return value;
            }
        }

        private static string ValueToString(object value)
            => (value switch
            {
                bool b => b ? "true" : "false",
                _ => value
            }).ToString();

        private static string DeserializeToQuery<T>(T obj)
        {
            if (obj is null) return null;
            var typeooft = obj.GetType();
            var type = typeooft.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var query = type.Select(it => new
                {
                    it.Name, Value = typeooft.GetField(it.Name)?.GetValue(obj)
                })
                .Where(it => !(it.Value is null))
                .Select(x => $"{WebUtility.UrlEncode(x.Name)}={WebUtility.UrlEncode(ValueToString(x.Value))}");
            return string.Join("&", query);
        }

        public async Task<TResult> Request<TResult, TSettings>(string apiMethod, TSettings settings,
            AccessToken token = null, string method = "GET", bool userJsonSerializer = false, string paramName = default)
        {
            var destination = $"{Site}{apiMethod}" + method switch
            {
                "GET" => "?" + DeserializeToQuery(settings),
                "HEAD" => "?" + DeserializeToQuery(settings),
                _ => string.Empty
            };
            var settingsInfo = method switch
            {
                "GET" => default,
                "HEAD" => default,
                _=> DeserializeToRequest(settings, userJsonSerializer, paramName)
            };
            return await _apiClient.RequestForm<TResult>(destination, settingsInfo, token, method);
        }

        private static async Task<string> SerializeToJson(object obj)
        {
            return await Task.Factory.StartNew(() => JsonConvert.SerializeObject(obj));
        }

        public async Task<TResult> SendJson<TResult>(string apiMethod, object content, AccessToken token,
            string method = "POST")
        {
            var json = new StringContent(await SerializeToJson(content), Encoding.UTF8, "application/json");
            return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", json, token, method);
        }

        public async Task<TResult> Request<TResult>(string apiMethod, AccessToken token = null, string method = "GET")
        {
            return await _apiClient.RequestForm<TResult>($"{Site}{apiMethod}", token);
        }

        public async Task NoResponseRequest(string apiMethod, AccessToken token, string method = "POST")
        {
            await _apiClient.RequestWithNoResponse($"{Site}{apiMethod}", null, token, method);
        }

        public async Task NoResponseRequest<TSettings>(string apiMethod, TSettings setting, AccessToken token, string method = "POST")
        {
            var settings = DeserializeToRequest(setting);
            await _apiClient.RequestWithNoResponse($"{Site}{apiMethod}", settings, token);
        }
    }

    public enum Version
    {
        v1,
        v2
    }
}