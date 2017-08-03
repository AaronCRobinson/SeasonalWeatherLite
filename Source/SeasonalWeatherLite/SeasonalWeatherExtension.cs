using Harmony;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace SeasonalWeatherLite
{
    public class SeasonalWeatherExtension : DefModExtension
    {
        public List<WeatherCommonalityRecord> spring = new List<WeatherCommonalityRecord>();
        public List<WeatherCommonalityRecord> summer = new List<WeatherCommonalityRecord>();
        public List<WeatherCommonalityRecord> fall = new List<WeatherCommonalityRecord>();
        public List<WeatherCommonalityRecord> winter = new List<WeatherCommonalityRecord>();
    }

    [StaticConstructorOnStartup]
    class SeasonalWeatherExtensionPatches
    {
        static SeasonalWeatherExtensionPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.whyisthat.seasonalweatherlite.seasonalweatherextension");

            harmony.Patch(AccessTools.Method(typeof(DateNotifier), nameof(DateNotifier.DateNotifierTick)), new HarmonyMethod(typeof(SeasonalWeatherExtensionPatches), nameof(DateNotifierTickPrefix)), null);
        }

        public static void DateNotifierTickPrefix(DateNotifier __instance)
        {
            Traverse t = Traverse.Create(__instance);

            // copy pasta (RimWorld.DateNotifier)
            Map map = t.Method("FindPlayerHomeWithMinTimezone").GetValue<Map>();
            float latitude = (map == null) ? 0f : Find.WorldGrid.LongLatOf(map.Tile).y;
            float longitude = (map == null) ? 0f : Find.WorldGrid.LongLatOf(map.Tile).x;
            Season season = GenDate.Season((long)Find.TickManager.TicksAbs, latitude, longitude);

            Season lastSeason = t.Field("lastSeason").GetValue<Season>();
            if (season != lastSeason && (lastSeason == Season.Undefined || season != lastSeason.GetPreviousSeason()))
            {
                //Log.Message("SeasonalWeather: season change");
                switch (season)
                {
                    case Season.Spring:
                        map.Biome.baseWeatherCommonalities = map.Biome.GetModExtension<SeasonalWeatherExtension>().spring;
                        break;
                    case Season.Summer:
                        map.Biome.baseWeatherCommonalities = map.Biome.GetModExtension<SeasonalWeatherExtension>().summer;
                        break;
                    case Season.Fall:
                        map.Biome.baseWeatherCommonalities = map.Biome.GetModExtension<SeasonalWeatherExtension>().fall;
                        break;
                    case Season.Winter:
                        map.Biome.baseWeatherCommonalities = map.Biome.GetModExtension<SeasonalWeatherExtension>().winter;
                        break;
                }
            }

        }
    }
}
