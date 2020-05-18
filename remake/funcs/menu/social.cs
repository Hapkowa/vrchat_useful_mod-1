using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MelonLoader;
using hashmod;
using VRC;
using VRC.Core;
using VRCSDK2;
using hashmod.remake.util;
using Transmtn.DTO.Notifications;
using Transmtn.DTO;

namespace hashmod.remake.funcs.menu
{
    public class social
    {
        public static void send_msg_to_social()
        {
            var ff = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = ff.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            MelonModLogger.Log("user selected " + userInfo.displayName + " id " + userInfo.user.id);

            if (Time.time > hashmod.last_msg_apicall)
            {
                hashmod.last_msg_apicall = Time.time + 30;
                hashmod.in_input_shit = true;
                menu.input_text("Enter the text to send", "A message to send to the target", new Action<string>((a) =>
                {
                    hashmod.in_input_shit = false;
                    VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.field_Private_Api_0.PostOffice.Send(Invite.Create(userInfo.user.id, "", new Location("", new Transmtn.DTO.Instance("", userInfo.user.id, "", "", "", false)), a));
                }));
            }
            else
            {
                hashmod.in_input_shit = false;

                var sec_left = hashmod.last_msg_apicall - Time.time;
                hashmod.error_type_poput("Function is still on cooldown!", "Please wait " + Math.Floor(sec_left) + " seconds before trying again!");
            }
        }
        public static void log_asset_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = utils.get_player(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found id " + userInfo.user.id);
                return;
            }

            MelonModLogger.Log("Asset for user " + userInfo.user.displayName + " -> " + found_player.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.assetUrl);
            MelonModLogger.Log("Avatar ID: " + found_player.field_Private_VRCAvatarManager_0.field_Private_ApiAvatar_0.id);
            MelonModLogger.Log("User ID: " + userInfo.user.id);
        }
        public static void do_clone_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = utils.get_player(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found");
                return;
            }
            if (found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus != "public")
            {
                MelonModLogger.Log("Avatar cloning failed, avatar is not public! (" + found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0.releaseStatus + ")");
                return;
            }

            MelonModLogger.Log("Attempting clone for user " + userInfo.user.displayName.ToString());

            var avatar_menu = GameObject.Find("Screens").transform.Find("Avatar").GetComponent<VRC.UI.PageAvatar>();
            avatar_menu.avatar.field_Internal_ApiAvatar_0 = found_player.prop_VRCAvatarManager_0.field_Private_ApiAvatar_0;
            avatar_menu.ChangeToSelectedAvatar();

            MelonModLogger.Log("Done!");
        }
        public static void do_tp_to_social()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = utils.get_player(userInfo.user.id);
            if (found_player == null)
            {
                MelonModLogger.Log("player could not be found");
                return;
            }
            var self = utils.get_local();
            if (self == null)
            {
                MelonModLogger.Log("local could not be found");
                return;
            }
            VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = found_player.transform.position;
            MelonModLogger.Log("Done");
        }
    }
}
