using UnityEngine;

namespace Game.Utils
{
    public static class Utils
    {
        
        public static bool CheckChance(float chanceGet)
        {
            var res = Random.Range(0f, 100f);
            return res < chanceGet;
        }
    }
}