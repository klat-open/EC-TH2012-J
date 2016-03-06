using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections;
public class NL_Checkout 
{ 
    private String nganluong_url = "http://sandbox.nganluong.vn/checkout.php";
    private String merchant_site_code = "275";    //thay ma merchant site ma ban da dang ky vao day 
    private String secure_pass = "123456";    //thay mat khau giao tiep giua website cua ban voi NganLuong.vn ma ban da dang ky vao day//91f020425a901101 
    public String GetMD5Hash(String input) 
    { 
        System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(input); 
        bs = x.ComputeHash(bs); 
        System.Text.StringBuilder s = new System.Text.StringBuilder();
        foreach (byte b in bs) 
        { 
            s.Append(b.ToString("x2").ToLower()); 
        } 
        String md5String = s.ToString();
        return md5String; } 
    public String buildCheckoutUrl(String return_url, String receiver, String transaction_info, String order_code, String price) 
    { 
        // Tạo biến secure code 
        String secure_code = ""; 
        secure_code += this.merchant_site_code; 
        secure_code += " " + HttpUtility.UrlEncode(return_url).ToLower(); 
        secure_code += " " + receiver; 
        secure_code += " " + transaction_info; 
        secure_code += " " + order_code; 
        secure_code += " " + price;
        secure_code += " " + this.secure_pass;
        // Tạo mảng băm 
        Hashtable ht = new Hashtable(); 
        ht.Add("merchant_site_code", this.merchant_site_code); 
        ht.Add("return_url", HttpUtility.UrlEncode(return_url).ToLower());
        ht.Add("receiver", receiver);
        ht.Add("transaction_info", transaction_info); 
        ht.Add("order_code", order_code); 
        ht.Add("price", price); 
        ht.Add("secure_code", this.GetMD5Hash(secure_code));
        // Tạo url redirect 
        String redirect_url = this.nganluong_url;
        if (redirect_url.IndexOf("?") == -1) 
        { 
            redirect_url += "?"; 
        } 
        else if (redirect_url.Substring(redirect_url.Length - 1, 1) != "?" && redirect_url.IndexOf("&") == -1) 
        { 
            redirect_url += "&"; 
        } 
        String url = "";
        // Duyêt các phần tử trong mảng băm ht dể tạo redirect url 
        IDictionaryEnumerator en = ht.GetEnumerator();
        while (en.MoveNext()) 
        { 
            if (url == "") url += en.Key.ToString() + "=" + en.Value.ToString();
            else url += "&" + en.Key.ToString() + "=" + en.Value;
        }
        String rdu = redirect_url + url;return rdu;
    } 
    public Boolean verifyPaymentUrl(String transaction_info, String order_code, String price, String payment_id, String payment_type, String error_text, String secure_code) 
    { 
        // Tạo mã xác thực từ web 
        String str = ""; 
        str += " " + transaction_info;
        str += " " + order_code; 
        str += " " + price; 
        str += " " + payment_id;
        str += " " + payment_type; 
        str += " " + error_text;
        str += " " + this.merchant_site_code;
        str += " " + this.secure_pass;
        // Mã hóa các tham số
        String verify_secure_code = ""; 
        verify_secure_code = this.GetMD5Hash(str);
        // Xác thực mã của web với mã trả về từ nganluong.vn 
        if (verify_secure_code == secure_code) return true;return false; 
    } 
    protected void btnSubmit_Click(object sender, EventArgs e) 
    { 
        String return_url = "http://localhost:35921/WebSite/"; 
        String transaction_info = "DEMO"; 
        String order_code = DateTime.Now.ToString("yyyyMMddHHmmss");
        String receiver = "webmaster@dotnet.vn";
        //Tài khoản nhận tiền 
        String price = "100000";
        NL_Checkout nl = new NL_Checkout();
        String url; url = nl.buildCheckoutUrl(return_url, receiver, transaction_info, order_code, price); 
        //Response.Redirect(url); 
    } 

}

