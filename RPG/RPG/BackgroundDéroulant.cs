//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace AtelierXNA
//{
//   public class BackgroundDÈroulant : Microsoft.Xna.Framework.DrawableGameComponent
//   {
//      Atelier Jeu { get; set; }
//      Texture2D ImageDeFond { get; set; }
//      string NomImage { get; set; }
//      float IntervalleMAJ { get; set; }
//      float Temps…coulÈDepuisMAJ { get; set; }
//      float …chelle { get; set; }
//      Vector2 Position…cran { get; set; }
//      Vector2 PositionOrigine { get; set; }
//      Vector2 TailleImage { get; set; }

//      public BackgroundDÈroulant(Atelier jeu, string nomImage, float intervalleMAJ)
//         : base(jeu)
//      {
//         Jeu = jeu;
//         NomImage = nomImage;
//         IntervalleMAJ = intervalleMAJ;
//      }

//      public override void Initialize()
//      {
//         Temps…coulÈDepuisMAJ = 0;
//         base.Initialize();
//      }

//      protected override void LoadContent()
//      {
//         ImageDeFond = Jeu.GestionnaireDeTextures.Find(NomImage);
//         PositionOrigine = Vector2.Zero;
//         Position…cran = new Vector2(Jeu.Window.ClientBounds.Width / 2, 0);
//         …chelle = MathHelper.Max(Jeu.Window.ClientBounds.Width / (float)ImageDeFond.Width,
//                                  Jeu.Window.ClientBounds.Height / (float)ImageDeFond.Height);
//         TailleImage = new Vector2(ImageDeFond.Width * …chelle, 0);
//      }

//      public override void Update(GameTime gameTime)
//      {
//         this.Visible = Jeu.CamÈraJeu.Position.Y > 0;
//         float Temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
//         Temps…coulÈDepuisMAJ += Temps…coulÈ;
//         if (Temps…coulÈDepuisMAJ >= IntervalleMAJ)
//         {
//            float deltaX = 0.2f;
//            Position…cran = new Vector2((Position…cran.X + deltaX) % (ImageDeFond.Width * …chelle),
//                                        Position…cran.Y);
//            Temps…coulÈDepuisMAJ = 0;
//         }
//      }

//      public override void Draw(GameTime gameTime)
//      {
//         Jeu.GestionSprites.Begin();
//         if (Position…cran.X < Jeu.Window.ClientBounds.Width)
//         {
//             Jeu.GestionSprites.Draw(ImageDeFond, Position…cran, null, Color.White, 0,
//                                     PositionOrigine, …chelle, SpriteEffects.None, 1f);
//         }
//         Jeu.GestionSprites.Draw(ImageDeFond, Position…cran - TailleImage, null, Color.White,
//                                 0, PositionOrigine, …chelle, SpriteEffects.None, 1f);
//         Jeu.GestionSprites.End();
//      }
//   }
//}