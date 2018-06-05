using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GalaxyCommand.Code.Game.Services
{
    public class KeyBindingsService
    {

        public static KeyCode[] GroupList = new KeyCode[]
        {
            KeyBindingsService.Group1, KeyBindingsService.Group2, KeyBindingsService.Group3,
            KeyBindingsService.Group4, KeyBindingsService.Group5, KeyBindingsService.Group6,
            KeyBindingsService.Group7, KeyBindingsService.Group8, KeyBindingsService.Group8
        };

    public static KeyCode Group1
        {
            get { return KeyCode.Keypad1; }
        }
        public static KeyCode Group2
        {
            get { return KeyCode.Keypad2; }
        }
        public static KeyCode Group3
        {
            get { return KeyCode.Keypad3; }
        }
        public static KeyCode Group4
        {
            get { return KeyCode.Keypad4; }
        }
        public static KeyCode Group5
        {
            get { return KeyCode.Keypad5; }
        }
        public static KeyCode Group6
        {
            get { return KeyCode.Keypad6; }
        }
        public static KeyCode Group7
        {
            get { return KeyCode.Keypad7; }
        }
        public static KeyCode Group8
        {
            get { return KeyCode.Keypad8; }
        }
        public static KeyCode Group9
        {
            get { return KeyCode.Keypad9; }
        }
    }
}
