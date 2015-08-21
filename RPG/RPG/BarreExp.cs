using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BarreExp : Microsoft.Xna.Framework.DrawableGameComponent
    {
        RPG Jeu { get; set; }
        ScèneDeJeu SceneJeu { get; set; }

        Texture2D Texture { get; set; }
        string NomTexture { get; set; }

        Rectangle Destination { get; set; }
        int SourceX { get; set; }
        float Durée { get; set; }
        

        float TempsDepuisMAJ { get; set; }

        int LengthDifference
        {
            get
            {
                return Texture.Width - Destination.Width;
            }
        }

        float Intervalle
        {
            get
            {
                return Durée / LengthDifference; ;
            }
        }

        public BarreExp(RPG jeu, ScèneDeJeu scenejeu, string nomtexture, Rectangle destination, float durée)
            : base(jeu)
        {
            Jeu = jeu;
            SceneJeu = scenejeu;
            NomTexture = nomtexture;
            Destination = destination;
            Durée = durée;
            SourceX = 0;
        }


        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
            base.Initialize();
        }
        protected override void LoadContent()
        {
           Texture = SceneJeu.GestionnaireDeTextures.Find(NomTexture);
           base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            TempsDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsDepuisMAJ >= Intervalle)
            {
                SourceX = (SourceX + 1) % LengthDifference;
                TempsDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SceneJeu.GestionSprites.Begin();
            SceneJeu.GestionSprites.Draw(Texture, new Rectangle((int)(Destination.X * SceneJeu.Scale.X), (int)(Destination.Y * SceneJeu.Scale.Y), (int)(Destination.Width * (SceneJeu.BaldorLeBrave.PtsExp / SceneJeu.BaldorLeBrave.ExpProchainNiveau) * SceneJeu.Scale.X), (int)(Destination.Height * SceneJeu.Scale.Y)), new Rectangle(SourceX, 0, (int)(Destination.Width * (SceneJeu.BaldorLeBrave.PtsExp / SceneJeu.BaldorLeBrave.ExpProchainNiveau)), Destination.Height), Color.White);
            SceneJeu.GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
