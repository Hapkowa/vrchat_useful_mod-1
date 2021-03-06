using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using hashmod;
using VRC;
using VRC.Core;
using VRCSDK2;
using hashmod.remake.util;
using Il2CppSystem.Net;
using MelonLoader;

namespace hashmod.remake.funcs.game
{
    public class flying
    {
        public static void height_adjust()
        {
            if (hashmod.fly_mode_onpress == false)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    hashmod.fly_up = false;
                    hashmod.fly_down = !hashmod.fly_down;
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hashmod.fly_down = false;
                    hashmod.fly_up = !hashmod.fly_up;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Q)) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);
                if (Input.GetKey(KeyCode.E)) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);
            }

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") < 0f) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, (hashmod.flying_speed * Time.deltaTime) * (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * -1), 0f);
            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") > 0f) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, (hashmod.flying_speed * Time.deltaTime) * (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")), 0f);

            if (hashmod.fly_down) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position - new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);
            if (hashmod.fly_up) VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, hashmod.flying_speed * Time.deltaTime, 0f);

            //better directional movement
            if (Input.GetKey(KeyCode.W)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * hashmod.flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.right * -1f * hashmod.flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.forward * -1f * hashmod.flying_speed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D)) VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.right * hashmod.flying_speed * Time.deltaTime;
            
            var motion_com = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<VRCMotionState>();
            if (motion_com != null) motion_com.Method_Public_Void_3();            
        }
        public static void noclip()
        {
            if (hashmod.isNoclip) Physics.gravity = new Vector3(0, 0, 0);
            else
            {
                Physics.gravity = VRC_SceneDescriptor.Instance.gravity;
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<VRCMotionState>().Method_Public_Void_0();
            }

            Collider[] array = GameObject.FindObjectsOfType<Collider>();
            Component component = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponents<Collider>().FirstOrDefault<Component>();
            Collider[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                Collider collider = array2[i];
                bool flag = collider.GetComponent<PlayerSelector>() != null || collider.GetComponent<VRC_Pickup>() != null || collider.GetComponent<QuickMenu>() != null || collider.GetComponent<VRC_Station>() != null || collider.GetComponent<VRC_AvatarPedestal>() != null;
                if (flag)
                {
                    collider.enabled = true;
                }
                else
                {
                    bool flag2 = collider != component && ((hashmod.isNoclip && collider.enabled || (!hashmod.isNoclip && hashmod.noClipToEnable.Contains(collider.GetInstanceID()))));
                    if (flag2)
                    {
                        collider.enabled = !hashmod.isNoclip;
                        if (hashmod.isNoclip)
                        {
                            hashmod.noClipToEnable.Add(collider.GetInstanceID());
                        }
                    }
                }
            }
            bool flag3 = !hashmod.isNoclip;
            if (flag3)
            {
                hashmod.noClipToEnable.Clear();
            }
        }
    }
}
