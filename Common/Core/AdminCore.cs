using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core
{
    public class AdminCore:SGoodsDB
    {
        private static Dictionary<string, string> tokenCache = new Dictionary<string, string>();
        public static string MakeToken(string uid, string psw, int type)
        {
            TokenObj obj = new TokenObj();
            obj.uid = uid;
            obj.psw = psw;
            obj.type = type;
            obj.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            obj.randomstr = Guid.NewGuid().ToString().Replace("-", "");
            string strToken = JsonConvert.SerializeObject(obj);
            string Token = DES.EncryptDES(strToken);
            return Token;
        }
        public static TokenObj CheckToken(string Token, out int code)
        {
            code = MessageCode.UNKONWN;
            int token_last_day = 99999;
            TokenObj tokenObj = null;
            try
            {
                string token = DES.DecryptDES(Token);
                tokenObj = JsonConvert.DeserializeObject<TokenObj>(token);
            }
            catch (Exception ex)
            {
                code = MessageCode.ERROR_TOKEN_VALIDATE;
                return null;

            }




            //判断Token是否过期
            string date = tokenObj.timestamp;
            DateTime token_date = DateTime.Parse(date);
            TimeSpan sp = DateTime.Now - token_date;
            if (sp.Days > token_last_day)
            {
                code = MessageCode.ERROR_TOKEN_TIMEOUT;

            }
            //判断Token是否合法


            if (tokenCache.Keys.Contains(tokenObj.uid))
            {
                string oldToken = tokenCache[tokenObj.uid];
                if (oldToken == Token)
                {
                    code = MessageCode.SUCCESS;
                    return tokenObj;
                }
            }
            SGoodsDB db = new SGoodsDB();
            DataSet ds = db.ExeQuery("select uid,psw from [guser] where uid=@uid and psw=@psw",
                new SqlParameter("uid", tokenObj.uid),
                new SqlParameter("psw", tokenObj.psw));
            db.Close();
            if (ds == null)
            {

                code = MessageCode.ERROR_EXECUTE_SQL;
                return null;
            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                code = MessageCode.ERROR_TOKEN_VALIDATE;
                return null;

            }

            tokenCache[tokenObj.uid] = Token;

            code = MessageCode.SUCCESS;
            return tokenObj;
        }
    }
}
