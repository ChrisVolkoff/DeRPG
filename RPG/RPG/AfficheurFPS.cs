using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   public class AfficheurFPS : Microsoft.Xna.Framework.DrawableGameComponent
   {
      const int MARGE_BAS = 10;
      const int MARGE_DROITE = 15;

      const float INTERVALLE_MAJ = 1f;

      RPG Jeu { get; set; }
      string NomFont { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
      int CptFrames { get; set; }
      float ValFPS { get; set; }
      Vector2 PositionDroiteBas { get; set; }
      Vector2 PositionChaîne { get; set; }
      string ChaîneFPS { get; set; }
      Vector2 Dimension { get; set; }
      SpriteFont ArialFont { get; set; }

      public AfficheurFPS(RPG game, string nomFont)
         : base(game)
      {
         Jeu = game;
         NomFont = nomFont;
      }

      public override void Initialize()
      {
         TempsÉcouléDepuisMAJ = 0;
         ValFPS = 0;
         CptFrames = 0;
         ChaîneFPS = "";
         PositionDroiteBas = new Vector2(Jeu.Window.ClientBounds.Width - MARGE_DROITE,
                                         Jeu.Window.ClientBounds.Height - MARGE_BAS);
         base.Initialize();
      }

      protected override void LoadContent()
      {
         ArialFont = Jeu.GestionnaireDeFonts.Find(NomFont); 
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         ++CptFrames;
         TempsÉcouléDepuisMAJ += tempsÉcoulé;
         if (TempsÉcouléDepuisMAJ >= INTERVALLE_MAJ)
         {
            float oldValFPS = ValFPS;
            ValFPS = CptFrames / TempsÉcouléDepuisMAJ;
            if (oldValFPS != ValFPS)
            {
               ChaîneFPS = ValFPS.ToString("0");
               Dimension = ArialFont.MeasureString(ChaîneFPS);
               PositionChaîne = PositionDroiteBas - Dimension;
               Jeu.Window.Title = ChaîneFPS;
            }
            CptFrames = 0;
            TempsÉcouléDepuisMAJ = 0;
         }
         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         Jeu.GestionSprites.Begin();
         Jeu.GestionSprites.DrawString(ArialFont, ChaîneFPS, PositionChaîne, Color.Cyan, 0,
                                      Vector2.Zero , 1.0f, SpriteEffects.None, 0);
         Jeu.GestionSprites.End();
      }
   }
}