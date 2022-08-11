/*
 * Responsible for putting the hats in the world.
 * Making sure they appear in certain rooms.
 * Uses spawns.txt file, which is added in HatWorldMain.OnEnable()
 * Based on GrappleWorld mod
 * 
 * Interfaces with HatPhysical.Update()
 */

// default imports
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// physical objects library
using Fisobs;

namespace HatWorld
{
    sealed class HatPlacer
    {
        public static Dictionary<HatAbstract, PlacedObjectInfo> infos = new();

        // key: room id - value: [float x, float y, string hatType]
        // A room can have multiple hats, one object[] per hat
        public static Dictionary<string, List<object[]>> hatsByRoom = new();

        public const int placedObjectIndex = 1821433636;

        public static void AddSpawns(Stream stream)
        {
            StreamReader streamReader = new StreamReader(stream);
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                string[] array = text.Split(new char[]
                {
                    ','
                });
                string key = array[0].Trim();

                List<object[]> room_list;
                if (!HatPlacer.hatsByRoom.TryGetValue(key, out room_list))
                {
                    room_list = (HatPlacer.hatsByRoom[key] = new List<object[]>());
                }

                float x = float.Parse(array[1].Trim());
                float y = float.Parse(array[2].Trim());
                string hatType = array[3].Trim();
                room_list.Add(new object[] { x, y, hatType });
                Debug.Log("Hatworld room_list " + key + " " + room_list);
            }
        }

        public static void AddHooks()
        {
            On.Room.Loaded += Room_Loaded;
            On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
        }

        public static void Room_Loaded(On.Room.orig_Loaded orig, Room self)
        {
            bool firstTimeRealized = self.abstractRoom.firstTimeRealized;
            orig.Invoke(self);
            if (self.game == null)
            {
                return;
            }
            if (!firstTimeRealized)
            {
                return;
            }
            List<object[]> list;
            if (HatPlacer.hatsByRoom.TryGetValue(self.abstractRoom.name, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    StoryGameSession getStorySession = self.game.GetStorySession;
                    if (getStorySession == null || !getStorySession.saveState.ItemConsumed(self.world, false, self.abstractRoom.index, 1821433636 + i))
                    {
                        HatAbstract abstractHat = new HatAbstract(self.world, self.GetWorldCoordinate(new Vector2((float) list[i][0], (float) list[i][1])), self.game.GetNewID(), (string) list[i][2]);
                        PlacedObjectInfo value = new PlacedObjectInfo
                        {
                            origRoom = self.abstractRoom.index,
                            placedObjectIndex = 1821433636 + i,
                            minRegen = 108,
                            maxRegen = 108
                        };
                        HatPlacer.infos[abstractHat] = value;
                        Debug.Log("Hatworld place hat " + self.abstractRoom.name + " " + (string)list[i][2]);
                        self.abstractRoom.AddEntity(abstractHat);
                    }
                }
            }
        }

        public static void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig.Invoke(self);
            HatPlacer.infos.Clear();
        }
    }
}
