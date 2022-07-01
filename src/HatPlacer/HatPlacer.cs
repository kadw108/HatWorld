/*
 * Responsible for putting the hats in the world.
 * Making sure they appear in certain rooms.
 * Based on GrappleWorld mod
 * 
 * Interfaces with HatPhysical.Update()
 */

// default imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;

// physical objects library
using Fisobs;

namespace HatWorld
{
    public class HatPlacer
    {
        public static Dictionary<HatAbstract, PlacedObjectInfo> infos = new();

        // key: room id - value: x, y, HatType
        public static Dictionary<string, List<Vector3>> hatsByRoom = new();

        public const int placedObjectIndex = 1821433636;

        public static void writeData(String data)
        {
            using (StreamWriter writer = new StreamWriter("C:\\Users\\account\\Apps\\Steam\\steamapps\\common\\Rain World\\hatworlddebug.txt"))
            {
                writer.WriteLine(data);
            }
        }

        public static void OnEnable()
        {
            // Debug.Log doesn't work here for some reason so use writeData instead

            On.Room.Loaded += Room_Loaded;
            On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;

            StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("HatWorld.src.HatPlacer.spawns.txt"));
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                string[] array = text.Split(new char[]
                {
                    ','
                });
                string key = array[0].Trim();
                writeData("reading " + key);
                List<Vector3> list;
                if (!HatPlacer.hatsByRoom.TryGetValue(key, out list))
                {
                    list = (HatPlacer.hatsByRoom[key] = new List<Vector3>());
                }
                list.Add(new Vector3(float.Parse(array[1].Trim()), float.Parse(array[2].Trim()), int.Parse(array[3].Trim())));
            }
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
            List<Vector3> list;
            if (HatPlacer.hatsByRoom.TryGetValue(self.abstractRoom.name, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    StoryGameSession getStorySession = self.game.GetStorySession;
                    if (getStorySession == null || !getStorySession.saveState.ItemConsumed(self.world, false, self.abstractRoom.index, 1821433636 + i))
                    {
                        HatAbstract abstractHat = new HatAbstract(self.world, self.GetWorldCoordinate(new Vector2(list[i][0], list[i][1])), self.game.GetNewID(), (HatType) (int) list[i][2]);
                        PlacedObjectInfo value = new PlacedObjectInfo
                        {
                            origRoom = self.abstractRoom.index,
                            placedObjectIndex = 1821433636 + i,
                            minRegen = 7,
                            maxRegen = 10
                        };
                        HatPlacer.infos[abstractHat] = value;
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
