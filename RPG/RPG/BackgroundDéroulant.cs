//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace AtelierXNA
//{
//   public class BackgroundDéroulant : Microsoft.Xna.Framework.DrawableGameComponent
//   {
//      Atelier Jeu { get; set; }
//      Texture2D ImageDeFond { get; set; }
//      string NomImage { get; set; }
//      float IntervalleMAJ { get; set; }
//      float TempsÉcouléDepuisMAJ { get; set; }
//      float Échelle { get; set; }
//      Vector2 PositionÉcran { get; set; }
//      Vector2 PositionOrigine { get; set; }
//      Vector2 TailleImage { get; set; }

//      public BackgroundDéroulant(Atelier jeu, string nomImage, float intervalleMAJ)
//         : base(jeu)
//      {
//         Jeu = jeu;
//         NomImage = nomImage;
//         IntervalleMAJ = intervalleMAJ;
//      }

//      public override void Initialize()
//      {
//         TempsÉcouléDepuisMAJ = 0;
//         base.Initialize();
//      }

//      protected override void LoadContent()
//      {
//         ImageDeFond = Jeu.GestionnaireDeTextures.Find(NomImage);
//         PositionOrigine = Vector2.Zero;
//         PositionÉcran = new Vector2(Jeu.Window.ClientBounds.Width / 2, 0);
//         Échelle = MathHelper.Max(Jeu.Window.ClientBounds.Width / (float)ImageDeFond.Width,
//                                  Jeu.Window.ClientBounds.Height / (float)ImageDeFond.Height);
//         TailleImage = new Vector2(ImageDeFond.Width * Échelle, 0);
//      }

//      public override void Update(GameTime gameTime)
//      {
//         this.Visible = Jeu.CaméraJeu.Position.Y > 0;
//         float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
//         TempsÉcouléDepuisMAJ += TempsÉcoulé;
//         if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
//         {
//            float deltaX = 0.2f;
//            PositionÉcran = new Vector2((PositionÉcran.X + deltaX) % (ImageDeFond.Width * Échelle),
//                                        PositionÉcran.Y);
//            TempsÉcouléDepuisMAJ = 0;
//         }
//      }

//      public override void Draw(GameTime gameTime)
//      {
//         Jeu.GestionSprites.Begin();
//         if (PositionÉcran.X < Jeu.Window.ClientBounds.Width)
//         {
//             Jeu.GestionSprites.Draw(ImageDeFond, PositionÉcran, null, Color.White, 0,
//                                     PositionOrigine, Échelle, SpriteEffects.None, 1f);
//         }
//         Jeu.GestionSprites.Draw(ImageDeFond, PositionÉcran - TailleImage, null, Color.White,
//                                 0, PositionOrigine, Échelle, SpriteEffects.None, 1f);
//         Jeu.GestionSprites.End();
//      }
//   }
//}