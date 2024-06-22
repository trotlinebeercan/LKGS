using UnityEngine;
using UnityEngine.SceneManagement;

namespace LKGS
{
    //[HarmonyPatch(typeof(ScGameManager))]
    //[HarmonyPatch(nameof(ScGameManager.SetUpManagers))]
    public class TimePatches : MonoBehaviour
    {
        private static float fDefault_gameFpsTarget = 60.0f;
        private static float fDefault_realMinPerGameHr = 0.65f;

        public void UpdateValues(int modifier)
        {
            // don't try to access something that does not exist
            if (ScGameManager.Instance?.GetTimeManager() == null)
            {
                return;
            }

            var timeManager = ScGameManager.Instance.GetTimeManager();

            var newRealMinPerGameHr = fDefault_realMinPerGameHr * (float)modifier;
            timeManager.realMinPerGameHr.floatValue = newRealMinPerGameHr;

            var newGameSecPerRealSec = fDefault_gameFpsTarget / newRealMinPerGameHr;
            timeManager.gameSecPerRealSec = newGameSecPerRealSec;

            var rmpgh = timeManager.realMinPerGameHr.floatValue;
            var gsprs = timeManager.gameSecPerRealSec;
            Plugin.L($"new values: rmpgh={rmpgh} | gsprs={gsprs}");
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            UpdateValues(Plugin.TimeValue);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            UpdateValues(1);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            UpdateValues(Plugin.TimeValue);
        }
    }
}