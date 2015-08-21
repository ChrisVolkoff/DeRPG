using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace AtelierXNA
{
    public class InputManager : Microsoft.Xna.Framework.GameComponent
    {
        protected RPG Jeu;
        List<Keys> AnciennesTouches { get; set; }
        List<Keys> NouvellesTouches { get; set; }
        public KeyboardState ÉtatClavier { get; private set; }
        public MouseState AncienÉtatSouris { get; private set; }
        public MouseState ÉtatSouris { get; private set; }
        private float TempsÉcouléDepuisMAJ { get; set; }

        bool GaucheDejaVérifié { get; set; }
        bool DroitDejaVérifié { get; set; }

        public InputManager(RPG game)
            : base(game)
        {
            Jeu = game;
        }

        public override void Initialize()
        {
            AnciennesTouches = new List<Keys>();
            NouvellesTouches = new List<Keys>();
            ÉtatSouris = new MouseState();
            AncienÉtatSouris = ÉtatSouris;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsÉcouléDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
                DroitDejaVérifié = false;
                GaucheDejaVérifié = false;
                AnciennesTouches = NouvellesTouches;
                ÉtatClavier = Keyboard.GetState();
                NouvellesTouches = new List<Keys>();
                foreach (Keys key in Keyboard.GetState().GetPressedKeys())
                {
                    NouvellesTouches.Add(key);
                }

                ActualiserÉtatSouris();


                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public bool EstClavierActivé
        {
            get { return NouvellesTouches.Count > 0; }
        }

        public bool EstNouvelleTouche(Keys touche)
        {
            bool EstNouvelleTouche;
            EstNouvelleTouche = (AnciennesTouches.Contains(touche) && !NouvellesTouches.Contains(touche));
            return EstNouvelleTouche;
        }

        private void ActualiserÉtatSouris()
        {
            AncienÉtatSouris = ÉtatSouris;
            ÉtatSouris = Mouse.GetState();
            if (ÉtatSouris.LeftButton == ButtonState.Pressed)
            {
                Vector2 Position = GetPositionSouris();
            }
        }

        public bool EstNouveauClicDroit()
        {

            bool retour = ÉtatSouris.RightButton == ButtonState.Pressed && AncienÉtatSouris.RightButton == ButtonState.Released;
            return retour;
        }

        public bool EstNouveauClicGauche()
        {
            bool retour = (ÉtatSouris.LeftButton == ButtonState.Pressed && AncienÉtatSouris.LeftButton == ButtonState.Released); 
            return retour;
        }

        public Vector2 GetPositionSouris()
        {
            return new Vector2(ÉtatSouris.X, ÉtatSouris.Y);
        }

    }

}