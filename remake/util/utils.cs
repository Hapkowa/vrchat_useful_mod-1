using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http;
using System.Net;
using System.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine.Events;
using UnityEngine.UI;
using hashmod.remake.btn;
using OVR.OpenVR;
using System.Net.Cache;
using Transmtn.DTO.Notifications;
using Transmtn.DTO;

namespace hashmod.remake.util
{
    public static class utils
    {
        static public int owo = 0;
        //x=lr y=ud
        //no idea how this shit works tbh somehow it will always copy the original objects state no matter what I do, if anyone can fix this pls tell me
        public static GameObject make_checkbox_ded(int x, int y, string txt, bool def, GameObject parent, Action<bool> act)
        {
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/SkipGoButtonInLoad").gameObject;
            var sss = UnityEngine.Object.Instantiate(BtnObj, BtnObj.transform, true);

            var Btn = sss.GetComponent<UnityEngine.UI.Toggle>();
            Btn.onValueChanged = new Toggle.ToggleEvent();
            Btn.transform.localPosition = new Vector3(parent.transform.localPosition.x - 275 + 100 + x, parent.transform.localPosition.y - 50 + 0 - y, parent.transform.localPosition.z);
            Btn.gameObject.SetActive(true);
            Btn.Set(def); Btn.isOn = def;
            Btn.transform.SetParent(parent.transform, false);
            Btn.transform.localPosition += new Vector3(150, 750);
            //Btn.GetComponentInChildren<UnityEngine.UI.Text>().text = txt;
            Btn.transform.localScale += new Vector3(2, 2, 0);

            return Btn.gameObject;
        }

        public static Text make_text_toggle(int x, int y, string txt, bool def, GameObject parent, Action<bool> act)
        {
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/SkipGoButtonInLoad").gameObject.GetComponent<UnityEngine.UI.Toggle>();

            var ds = new GameObject("qwertzzzz" + owo);
            ds.transform.SetParent(parent.transform, false);
            var text_thing = ds.AddComponent<Text>();

            text_thing.supportRichText = true;
            text_thing.text = "<b>" + txt + "</b>";
            text_thing.font = BtnObj.GetComponentInChildren<Text>().font;
            text_thing.fontStyle = BtnObj.GetComponentInChildren<Text>().fontStyle;
            text_thing.fontSize = 78;
            if (def == false) text_thing.color = Color.red;
            if (def == true) text_thing.color = Color.green;
            text_thing.gameObject.SetActive(true);

            text_thing.transform.localPosition = new Vector3(parent.transform.localPosition.x - 275 + 150 + x, parent.transform.localPosition.y - 65 - y, parent.transform.localPosition.z);
            text_thing.transform.localPosition += new Vector3(250, 2400, 0);
            text_thing.GetComponent<RectTransform>().sizeDelta = new Vector2(18 * 100, 100);

            var ck = text_thing.gameObject.AddComponent<Toggle>();
            ck.transform.localPosition -= new Vector3(50, 0, 0);
            ck.Set(def); ck.gameObject.SetActive(true);
            ck.onValueChanged = new Toggle.ToggleEvent();
            ck.onValueChanged.AddListener(act);
            ck.onValueChanged.AddListener(new Action<bool>((a) => //oh yeah also color management
            {
                if (a == false) text_thing.color = Color.red;
                if (a == true) text_thing.color = Color.green;
            }));

            owo++;
            return text_thing;
        }
        public static bool send_message(string msg, string id)
        {
            if (VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0 == null || VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.field_Private_Api_0 == null) return false;
            VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.field_Private_Api_0.PostOffice.Send(Invite.Create(id, "", new Location("", new Transmtn.DTO.Instance("", id, "", "", "", false)), msg));
            return true;
        }
        public static string convert(WebResponse res)
        {
            string strResponse = "";
            using (var stream = res.GetResponseStream())
            using (var reader = new StreamReader(stream)) strResponse = reader.ReadToEnd();
            res.Dispose();
            return strResponse;
        }
        public static string[] to_array(WebResponse res)
        {
            var str = convert(res);
            string[] strarr = str.Split(Environment.NewLine.ToCharArray());
            return strarr;
        }
        public static int check_version()
        {
            var client = WebRequest.Create("https://raw.githubusercontent.com/kichiro1337/vrchat_useful_mod/master/version.txt");
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            client.CachePolicy = noCachePolicy;

            ServicePointManager.ServerCertificateValidationCallback = (System.Object s, X509Certificate c, X509Chain cc, SslPolicyErrors ssl) => true;

            var response = convert(client.GetResponse());
            if (response.Contains(hashmod.mod_version) == false)
            {
                MelonModLogger.Log("!!! There was a update for this mod !!!");
                MelonModLogger.Log("!!! Please update the mod to enjoy new features and bug fixes !!!");
                MelonModLogger.Log("https://github.com/kichiro1337/vrchat_useful_mod");
                return int.Parse(response);
            }
            else
            {
                MelonModLogger.Log("Mod is up to date!");
                return int.Parse(response);
            }
        }
        public static string[] get_shader_blacklist()
        {
            var client = WebRequest.Create("https://raw.githubusercontent.com/kichiro1337/vrchat_useful_mod/master/blacklist-shaders.txt");
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            client.CachePolicy = noCachePolicy;

            ServicePointManager.ServerCertificateValidationCallback = (System.Object s, X509Certificate c, X509Chain cc, SslPolicyErrors ssl) => true;

            var response = to_array(client.GetResponse());
            response = response.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return response;
        }
        public static VRCPlayer get_local()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }
        public static Il2CppSystem.Collections.Generic.List<Player> get_all_player()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null) return null;
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }
        public static bool is_friend(Player p)
        {
            if (hashmod.friend_list.Contains(p.field_Private_APIUser_0.id)) return true;
            return false;
        }
        public static APIUser get_api(this Player p)
        {
            return p.field_Private_APIUser_0;
        }
        public static Player get_player(string id)
        {
            var t = get_all_player();
            for (var c = 0; c < t.Count; c++)
            {
                var p = t[c]; if (p == null) continue;
                if (p.get_api().id == id) return p;
            }
            return null;
        }
        public static Player get_player(int local_id)
        {
            var t = get_all_player();
            return t[local_id];
        }
        public static Player get_selected_player(this QuickMenu inst)
        {
            if (QuickMenu.prop_QuickMenu_0 == null ||
                QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0 == null ||
                PlayerManager.prop_PlayerManager_0 == null) return null;
            return get_player(QuickMenu.prop_QuickMenu_0.field_Private_APIUser_0.id);
        }
        public static QuickMenu get_quick_menu()
        {
            return QuickMenu.prop_QuickMenu_0;
        }
        public static PlayerManager get_player_manager()
        {
            return PlayerManager.prop_PlayerManager_0;
        }
        public static VRCUiManager get_ui_manager()
        {
            return VRCUiManager.prop_VRCUiManager_0;
        }
        public static UserInteractMenu get_interact_menu()
        {
            return Resources.FindObjectsOfTypeAll<UserInteractMenu>()[0];
        }
        public static void toggle_outline(Renderer render, bool state)
        {
            if (HighlightsFX.prop_HighlightsFX_0 == null) return;
            HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(render, state);
        }
        public static string get_instance_id()
        {
            return APIUser.CurrentUser.location;
        }
        public static void set_tooltip(this UiTooltip t)
        {
            var a = t.gameObject.GetComponentInChildren<UiToggleButton>();
            if (a == null) return;
            if (string.IsNullOrEmpty(t.alternateText)) return;
            var text = (!a.toggledOn) ? t.alternateText : t.text;
            if (TooltipManager.field_Private_Static_Text_0 != null) TooltipManager.Method_Public_Static_Void_String_2(text);
            if (t.tooltip != null) t.tooltip.text = text;
        }
        public static Text make_slider(GameObject parent, Action<float> act, int bx, int by, string text, float def, float max, float min, int negate)
        {
            var btn = btn_utils.create_btn(false, ButtonType.Default, "slider_element_" + bx + by, "", Color.white, Color.white, bx, by, parent.transform, null); btn.SetActive(false);
            var slider = UnityEngine.Object.Instantiate<Transform>(utils.get_ui_manager().menuContent.transform.Find("Screens/Settings/AudioDevicePanel/VolumeSlider"), parent.gameObject.transform);
            slider.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            slider.transform.localPosition = btn.gameObject.transform.localPosition; slider.transform.localPosition -= new Vector3(0, negate);
            slider.GetComponentInChildren<RectTransform>().anchorMin += new Vector2(0.06f, 0f);
            slider.GetComponentInChildren<RectTransform>().anchorMax += new Vector2(0.1f, 0f);
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged = new UnityEngine.UI.Slider.SliderEvent();
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().value = def;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = max;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().minValue = min;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged.AddListener(DelegateSupport.ConvertDelegate<UnityAction<float>>(act));
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().UpdateVisuals();
            var txt = new GameObject("Text"); txt.transform.SetParent(parent.transform, false);
            var txt_component = txt.AddComponent<Text>();
            txt_component.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); txt_component.fontSize = 64; txt_component.text = text;
            txt_component.transform.localPosition = slider.transform.localPosition;
            txt_component.transform.localPosition += new Vector3(txt_component.fontSize * text.Count() / 5, 75);
            txt_component.enabled = true;
            txt_component.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_component.fontSize * text.Count(), 100);
            txt_component.alignment = TextAnchor.MiddleLeft;
            return txt_component;
        }
    }
}
