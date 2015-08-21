using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class HealthBar : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int HAUTEUR_BARRE = 35;

        Combattant Sujet { get; set; }
        RPG Jeu { get; set; }
        Texture2D HealthTex { get; set; }
        Texture2D BackgroundTex { get; set; }
        string NomHealthTex { get; set; }
        string NomBackgroundTex { get; set; }
        SpriteFont Font { get; set; }
        string NomFont { get; set; }
        public bool ToRemove
        {
            get
            {
                return Sujet.ToRemove;
            }
        }

        ScèneDeJeu ScèneJeu { get; set; }

        public HealthBar(RPG jeu, ScèneDeJeu scenejeu, Monstre sujet, string nomhealthtex, string nombackgroundtex, string nomfont)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scenejeu;
            Sujet = sujet;
            NomHealthTex = nomhealthtex;
            NomBackgroundTex = nombackgroundtex;
            NomFont = nomfont;
            Visible = false;
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
           HealthTex = ScèneJeu.GestionnaireDeTextures.Find(NomHealthTex);
           BackgroundTex = ScèneJeu.GestionnaireDeTextures.Find(NomBackgroundTex);
           Font = ScèneJeu.GestionnaireDeFonts.Find(NomFont);

           base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Jeu.GestionSprites.Begin();
            Rectangle rectangleSourceVie = new Rectangle(0, 0, (int)(HealthTex.Width*(Sujet.PtsVie/Sujet.PtsVieMax)), HealthTex.Height);
            Jeu.GestionSprites.Draw(BackgroundTex, new Vector2((Jeu.Window.ClientBounds.Width - BackgroundTex.Width) / 2, HAUTEUR_BARRE-(BackgroundTex.Height/2)), Color.White);
            Jeu.GestionSprites.Draw(HealthTex, new Rectangle((Jeu.Window.ClientBounds.Width - HealthTex.Width) / 2, HAUTEUR_BARRE - (HealthTex.Height/ 2), (int)(HealthTex.Width * (Sujet.PtsVie / Sujet.PtsVieMax)), HealthTex.Height), rectangleSourceVie, Color.White);
            Jeu.GestionSprites.DrawString(Font, Sujet.Name, new Vector2((Jeu.Window.ClientBounds.Width - Font.MeasureString(Sujet.Name).X)/2, HAUTEUR_BARRE - (Font.MeasureString(Sujet.Name).Y/2)), Color.Wheat);    
            Jeu.GestionSprites.End();
            base.Draw(gameTime);
        }
    }

    public class HealthBarManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        RPG Jeu { get; set; }
        ScèneDeJeu ScèneJeu { get; set; }

        string NomHealthTexture { get; set; }
        string NomBackgroundTexture { get; set; }
        string NomFont { get; set; }

        public List<HealthBar> ListeDeBarres { get; private set; }


        public HealthBarManager(RPG jeu, ScèneDeJeu scenejeu, string nomhealthtexture, string nombackgroundtexture, string nomfont)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scenejeu;
            NomHealthTexture = nomhealthtexture;
            NomBackgroundTexture = nombackgroundtexture;
            NomFont = nomfont;
            ListeDeBarres = new List<HealthBar>();
        }

        public HealthBar AjouterBarreDeVie(Monstre sujet)
        {
            HealthBar tempBarre = new HealthBar(Jeu, ScèneJeu, sujet, NomHealthTexture, NomBackgroundTexture, NomFont);
            ListeDeBarres.Add(tempBarre);
            tempBarre.Initialize();
            return tempBarre;
        }


        public override void Initialize()
        {   
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            CheckForBarsToRemove();
            UpdateBars(gameTime);

            base.Update(gameTime);
        }

        private void CheckForBarsToRemove()
        {
           for (int i = ListeDeBarres.Count - 1; i >= 0; --i)
           {
              if (ListeDeBarres[i].ToRemove)
              {
                 ListeDeBarres.RemoveAt(i);
              }
           }
        }

        private void UpdateBars(GameTime gameTime)
        {
            foreach (HealthBar barre in ListeDeBarres)
            {
                barre.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (HealthBar barre in ListeDeBarres)
            {
                if (barre.Visible)
                {
                    barre.Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }
    }
}
