using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hashmod.remake.funcs.game;
using hashmod.remake.util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using Il2CppSystem.IO;
using VRC.Core;
using Transmtn;

//  todo note to optional fetch list and remove self from filtering shaders

namespace hashmod.remake.funcs.menu
{
    public class shader_menu
    {
        public static List<string> tmp_list = new List<string>();
        public static List<List<string>> pages = new List<List<string>>();
        private static List<GameObject> list_objects = new List<GameObject>();
        public static Player set_player;
        private static bool did_setup_button = false;
        public static Button next_page_button;
        public static int current_selected_page = 0;
        public static bool is_known(string shader)
        {
            if (hashmod.should_use_fetched_list == true) if (anticrash.shader_list.Contains(shader)) return true;
            if (anticrash.shader_list_local.Contains(shader)) return true;            
            return false;
        }
        public static bool is_playing(string shader)
        {
            var active_renderers = set_player.GetComponentsInChildren<Renderer>(false);
            for (var i=0;i<active_renderers.Count;i++)
            {
                var obj = active_renderers[i];
                if (obj == null) continue;
                for(var c=0;c<obj.materials.Count;c++)
                {
                    var jbo = obj.materials[c];
                    if (jbo == null) continue;
                    if (jbo.shader.name.Contains(shader)) return true;
                }
            }
            return false;
        }
        static public void set_page(int page)
        {
            var startpos = -300;
            var asdasd = GameObject.Find("Screens");

            foreach (var obj in list_objects) UnityEngine.Object.Destroy(obj);
            list_objects.Clear();
            foreach (var obj in pages[page])
            {
                startpos = page_display_routine(startpos, asdasd, obj);
            }
            next_page_button.enabled = true;
        }
        static public void go_next_page()
        {
            /*loop back to start if we are the end it doesnt even matter*/
            current_selected_page++; if (current_selected_page > pages.Count - 1) current_selected_page = 0;
            set_page(current_selected_page);
        }
        static public bool setup_listing()
        {
            var renderers = set_player.prop_VRCAvatarManager_0.GetComponentsInChildren<Renderer>(true);
            var startpos = -300;
            var asdasd = GameObject.Find("Screens");

            for (var i = 0; i < renderers.Count; i++)
            {
                var obj = renderers[i];
                if (obj == null) continue;
                for (var m = 0; m < obj.materials.Count; m++)
                {
                    var mat = obj.materials[m];
                    if (tmp_list.Contains(mat.shader.name) == false)
                    {
                        bool contained = false;
                        foreach (var a in pages) if (a.Contains(mat.shader.name)) { contained = true; break; }
                        if (contained == true) continue;
                        if (tmp_list.Count == 9)
                        {
                            //split to a new page
                            pages.Add(tmp_list);
                            tmp_list = new List<string>();
                        }
                        tmp_list.Add(mat.shader.name);

                    }
                }
            }
            if (tmp_list.Count != 0)
            {
                //leftovers go to the last page
                pages.Add(tmp_list);
                tmp_list = new List<string>();
            }
            MelonModLogger.Log("created pages " + pages.Count());
            foreach (var obj in pages.First())
            {
                startpos = page_display_routine(startpos, asdasd, obj);
            }
            next_page_button.gameObject.SetActive(true);
            next_page_button.enabled = true;
            return true;
        }

        private static int page_display_routine(int startpos, GameObject asdasd, string obj)
        {
            var txt = new GameObject("TextListing");
            txt.transform.SetParent(asdasd.transform, false);

            txt.layer = 1;
            txt.AddComponent<RectTransform>();

            var txt_component = txt.AddComponent<Button>();
            txt_component.gameObject.AddComponent<Text>();

            var text_part = txt_component.GetComponent<Text>(); text_part.transform.SetParent(asdasd.transform, false);
            text_part.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); text_part.fontSize = 42; text_part.text = obj;
            text_part.transform.localPosition = txt.transform.localPosition; text_part.transform.localPosition += new Vector3(0, 600, 0);
            text_part.transform.localPosition += new Vector3((text_part.text.Count() * 42) / 4, startpos, 0);
            text_part.transform.localRotation = asdasd.transform.localRotation;
            text_part.enabled = true;
            if (is_known(obj)) text_part.color = Color.red;
            else if (is_playing(obj)) text_part.color = Color.green;
            text_part.GetComponent<RectTransform>().sizeDelta = new Vector2(text_part.text.Count() * 42, 50);
            txt_component.GetComponent<RectTransform>().sizeDelta = new Vector2(text_part.text.Count() * 42, 50);
            txt_component.onClick.AddListener(new Action(() =>
            {
                var self = txt_component.GetComponent<Text>();
                MelonModLogger.Log("has clicked object \"" + self.text + "\"");
                if (anticrash.shader_list_local.Contains(self.text) == false)
                {
                    anticrash.shader_list_local.Add(self.text);
                    System.IO.File.WriteAllLines("hashmod_shaderlist.txt", anticrash.shader_list_local);
                    anticrash.shader_check(utils.get_quick_menu().get_selected_player());
                    //MelonModLogger.Log("added shader to local blacklist \"" + self.text + "\"");
                    self.color = Color.red;
                }
                else
                {
                    self.color = Color.white;
                    anticrash.shader_list_local.Remove(self.text);
                    System.IO.File.WriteAllLines("hashmod_shaderlist.txt", anticrash.shader_list_local);
                    //MelonModLogger.Log("removing shader from local blacklist \"" + self.text + "\"");
                }
            }));

            txt.GetComponent<RectTransform>().sizeDelta = new Vector2(text_part.text.Count() * 42, 50);

            list_objects.Add(txt);

            startpos -= 50;
            return startpos;
        }

        static public void reset_all()
        {
            tmp_list.Clear();
            pages.Clear();
            set_player = null;
            current_selected_page = 0;
            next_page_button.enabled = false;
            next_page_button.gameObject.SetActive(false);
            if (list_objects.Count != 0) foreach (var o in list_objects) UnityEngine.Object.Destroy(o);
        }
        static private bool setup_back_button()
        {
            var base_object = GameObject.Find("UserInterface/QuickMenu");
            var page_up_btn = GameObject.Find("UserInterface/QuickMenu/EmojiMenu/PageUp");
            var clone_page_btn = UnityEngine.Object.Instantiate<GameObject>(page_up_btn.gameObject);

            var btn_object = clone_page_btn.GetComponent<Button>();
            btn_object.onClick = new Button.ButtonClickedEvent();
            btn_object.onClick.AddListener(new Action(() => { go_next_page(); }));

            btn_object.transform.localPosition -= new Vector3(50, 200);
            btn_object.transform.localRotation = base_object.transform.localRotation;
            var parent = GameObject.Find("Screens");

            next_page_button = btn_object;

            btn_object.enabled = false;
            btn_object.transform.SetParent(parent.transform, false);
            did_setup_button = true;
            next_page_button.gameObject.SetActive(false);

            parent.AddComponent<Canvas>();
            parent.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            parent.AddComponent<CanvasScaler>();
            parent.AddComponent<GraphicRaycaster>();
            parent.AddComponent<VRCSDK2.VRC_UiShape>();

            return true;
        }
        static public void work()
        {
            if (did_setup_button == false) setup_back_button();
            var cur_player = utils.get_quick_menu().get_selected_player();
            if (APIUser.CurrentUser == null || cur_player == null || cur_player.field_Private_APIUser_0 == null) return;
            if ((cur_player == null && set_player != null) || (APIUser.CurrentUser.id == cur_player.field_Private_APIUser_0.id && set_player != null))
            {
                MelonModLogger.Log("unselected player, clearing listing");
                reset_all();
                return;
            }
            if (set_player != cur_player && APIUser.CurrentUser.id != cur_player.field_Private_APIUser_0.id)
            {
                /*clear and swap shader list*/
                MelonModLogger.Log("setting new player shader data");
                reset_all();
                set_player = cur_player; /*setup the next player*/
                setup_listing();
                set_page(0);
            }
        }
    }
}


