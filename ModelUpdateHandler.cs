using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace UnityModelReplacement
{
    public static class ModelUpdateHandler
    {
        private static bool initialized = false;
        private static HashSet<BodyReplacementBase> bodyReplacements = new HashSet<BodyReplacementBase>();

        private static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            var rootSystem = PlayerLoop.GetCurrentPlayerLoop();
            rootSystem = rootSystem.InjectAfter<PreLateUpdate, PreLateUpdate.ScriptRunBehaviourLateUpdate>(
                new PlayerLoopSystem()
                {
                    updateDelegate = UpdateModelReplacements,
                    type = typeof(ModelUpdateHandler)
                });
            PlayerLoop.SetPlayerLoop(rootSystem);
            initialized = true;
        }

        private static void UpdateModelReplacements()
        {
            /*
            foreach (var builder in bodyReplacements)
            {
                builder.CustomUpdate();
            }
            */
        }

        private static PlayerLoopSystem InjectAfter<T,A>(this PlayerLoopSystem self, PlayerLoopSystem systemToInject)
        {
            // Have to do this silly index lookup because everything is an immutable struct and must be modified in-place.
            var preLateUpdateSystemIndex = FindIndexOfSubsystem<T>(self.subSystemList);
            if (preLateUpdateSystemIndex == -1)
            {
                throw new UnityException($"Failed to find PlayerLoopSystem with type{typeof(T)}");
            }
            var preLateUpdateSubSystemIndex = FindIndexOfSubSubsystem<A>(self.subSystemList[preLateUpdateSystemIndex].subSystemList);
            if (preLateUpdateSubSystemIndex == -1)
            {
                throw new UnityException($"Failed to find PlayerLoopSubSystem with type{typeof(T)}");
            }
            List<PlayerLoopSystem> preLateUpdateSubsystems = new List<PlayerLoopSystem>(self.subSystemList[preLateUpdateSystemIndex].subSystemList);
            foreach (PlayerLoopSystem loop in preLateUpdateSubsystems)
            {
                if (loop.type != systemToInject.type) continue;
                Debug.LogWarning($"Tried to inject a PlayerLoopSystem ({systemToInject.type}) more than once! Ignoring the second injection.");
                return self; // Already injected!!!
            }
            preLateUpdateSubsystems.Insert(preLateUpdateSubSystemIndex + 1,
                new PlayerLoopSystem()
                {
                    updateDelegate = UpdateModelReplacements,
                    type = typeof(ModelUpdateHandler)
                }
            );

            self.subSystemList[preLateUpdateSystemIndex].subSystemList = preLateUpdateSubsystems.ToArray();


            foreach (var item in self.subSystemList[preLateUpdateSystemIndex].subSystemList.ToList())
            {
                Console.WriteLine(item);
            }

            return self;
        }

        private static int FindIndexOfSubSubsystem<T>(PlayerLoopSystem[] list, int index = -1)
        {
            if (list == null) return -1;
            for (int i = 0; i < list.Length; i++)
            {
                if ((list[i].type == typeof(T)))
                {
                    return i;
                }
            }
            return -1;
        }

        private static int FindIndexOfSubsystem<T>(PlayerLoopSystem[] list, int index = -1)
        {
            if (list == null) return -1;
            for (int i = 0; i < list.Length; i++)
            {
                if ((list[i].type == typeof(T)) )
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
