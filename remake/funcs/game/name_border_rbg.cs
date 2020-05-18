using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hashmod.remake.util;
using UnityEngine;
using VRC.Core;

namespace hashmod.remake.funcs.game
{
    public class name_border_rbg
    {
        public static void name_border_clr()
        {
            var users = utils.get_all_player();
            if (users == null) return;
            for (var i = 0; i < users.Count; i++)
            {
                var obj = users[i];
                if (obj == null) continue;
                if (obj.field_Private_APIUser_0 == null) continue;
                if (obj.field_Private_APIUser_0.id.Contains(APIUser.CurrentUser.id)) continue;
                if (hashmod.friend_list.Contains(obj.field_Private_APIUser_0.id) == false) continue;             

                obj.field_Private_VRCPlayerApi_0.SetNamePlateColor(HSBColor.ToColor(new HSBColor(Mathf.PingPong(Time.time * 0.1f, 1), 1, 1)));
            }
        }
    }
}
