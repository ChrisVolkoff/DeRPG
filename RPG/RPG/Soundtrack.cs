using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System;

namespace AtelierXNA
{
    class PriorityWithName
    {
        public string CueName;
        public int Priority;
        public PriorityWithName(string name, int priority)
        {
            CueName = name;
            Priority = priority;
        }
    }

    static class Soundtrack
    {
        const float DIMINUTION_RANGE_SOUND_EFFECT = 50f;
        const float AMPLITUDE_DISTANCE_EFFECT = 2f;

        public static bool IsSongCueStopped { get { return (CurrentSongCue == null || CurrentSongCue.IsStopped); } }
        public static bool IsSoundCueStopped { get { return (CurrentSoundCue == null || CurrentSoundCue.IsStopped); } }

        private static Cue CurrentSongCue { get; set; }
        private static Cue CurrentSoundCue { get; set; }

        private static List<PriorityWithName> ListPriorité { get; set; }

        private static AudioEngine Engine { get; set; }
        private static SoundBank Sounds { get; set; }
        private static WaveBank Waves { get; set; }

        static Soundtrack()
        {
            Initialize();
            LoadContent();

            ListPriorité = new List<PriorityWithName>();
            FillPriorityList();
        }

        public static void Initialize()
        {
        }

        static void LoadContent()
        {
            // Music
            Engine = new AudioEngine("Content/Music/Project_v3.xgs");
            Sounds = new SoundBank(Engine, "Content/Music/Sound Bank.xsb");
            Waves = new WaveBank(Engine, "Content/Music/Wave Bank.xwb", 0, 16);

            Update();
        }

        public static void Update()
        {
            Engine.Update();
        }

        //  Plays a new SONG Cue. Stops old cue if there is one.
        public static void StartSongCue(string cueName)
        {
            StopSong();
            CurrentSongCue = Sounds.GetCue(cueName);
            CurrentSongCue.Play();
        }

        //  Plays a new SOUND Cue. Stops old cue if there is one.
        public static void StartSoundCue(string cueName)
        {
            if (CurrentSoundCue != null && CurrentSoundCue.IsStopped)
            {
                CurrentSoundCue = null;
            }
            if (CurrentSoundCue != null)
            {
                if (GetPriorityFromCueName(cueName) > GetPriorityFromCueName(CurrentSoundCue.Name))
                {
                    StopSound();
                    CurrentSoundCue = Sounds.GetCue(cueName);
                    CurrentSoundCue.Play();
                }
            }
            else
            {
                CurrentSoundCue = Sounds.GetCue(cueName);
                CurrentSoundCue.Play();
            }
        }

        public static void StartSongCueWithDistanceEffect(string cueName, string category, Vector3 listenerPos, Vector3 emmiterPos, float distMax)
        {
            StartSongCue(cueName);
            UpdateSongCueDistanceEffect(category, listenerPos, emmiterPos, distMax);
        }

        public static void UpdateSongCueDistanceEffect(string category, Vector3 listenerPos, Vector3 emmiterPos, float distMax)
        {
            float distance = ProjectileBalistique.DistanceEntreDeuxPoints3D(emmiterPos, listenerPos);
            float volume = AMPLITUDE_DISTANCE_EFFECT * Math.Abs(((distMax - DIMINUTION_RANGE_SOUND_EFFECT) - distance) / distMax);
            AdjustVolume(category, volume);
        }

        // STOP
        public static void StopSong()
        {
            if (CurrentSongCue != null)
            {
                CurrentSongCue.Stop(AudioStopOptions.AsAuthored);
                CurrentSongCue.Dispose();
                CurrentSongCue = null;
            }
        }

        public static void StopSound()
        {
            if (CurrentSoundCue != null)
            {
                CurrentSoundCue.Stop(AudioStopOptions.AsAuthored);
                CurrentSoundCue.Dispose();
                CurrentSoundCue = null;
            }
        }

        // Adjust the volume of a category
        public static void AdjustVolume(string category, float volume)
        {
            Engine.GetCategory(category).SetVolume(volume);
        }

        // Get the priority of a sound with its cue
        private static int GetPriorityFromCueName(string cueName)
        {
            PriorityWithName result = ListPriorité.Find(delegate(PriorityWithName i)
            {
                return i.CueName == cueName;
            }
            );
            return result.Priority;
        }

        // Fill list of priorities
        private static void FillPriorityList()
        {
            ListPriorité.Add(new PriorityWithName("boss_aggro", 150));
            ListPriorité.Add(new PriorityWithName("boss_death", 150));
            ListPriorité.Add(new PriorityWithName("boss_laugh", 120));
            ListPriorité.Add(new PriorityWithName("boss_minion", 150));
            ListPriorité.Add(new PriorityWithName("boss_win", 150));
            ListPriorité.Add(new PriorityWithName("melee_swing", 30));
            ListPriorité.Add(new PriorityWithName("minion_laugh", 80));
            ListPriorité.Add(new PriorityWithName("proj_explosion", 35));
            ListPriorité.Add(new PriorityWithName("proj_cast", 40));
            ListPriorité.Add(new PriorityWithName("nec_moremana", 45));
            ListPriorité.Add(new PriorityWithName("nec_death", 105));
            ListPriorité.Add(new PriorityWithName("sorc_death", 0));
            ListPriorité.Add(new PriorityWithName("button_click", 1));
            ListPriorité.Add(new PriorityWithName("levelup", 100));
            ListPriorité.Add(new PriorityWithName("cain_cube", 250));
            ListPriorité.Add(new PriorityWithName("wololo", 0));
            ListPriorité.Add(new PriorityWithName("m_menu", 255));
            ListPriorité.Add(new PriorityWithName("m_goldeneye", 255));
            ListPriorité.Add(new PriorityWithName("boss_battle_start", 254));
            ListPriorité.Add(new PriorityWithName("boss_battle_loop", 255));
            ListPriorité.Add(new PriorityWithName("m_act1", 255));
            ListPriorité.Add(new PriorityWithName("m_act2", 255));
            ListPriorité.Add(new PriorityWithName("m_act3", 255));
            ListPriorité.Add(new PriorityWithName("m_act4", 255));
            ListPriorité.Add(new PriorityWithName("m_act1_2", 255));
            ListPriorité.Add(new PriorityWithName("sorc_nova", 41));
            ListPriorité.Add(new PriorityWithName("chrono_start", 98));
            ListPriorité.Add(new PriorityWithName("chrono_loop", 99));
            ListPriorité.Add(new PriorityWithName("confusion", 42));
        }
    }
}