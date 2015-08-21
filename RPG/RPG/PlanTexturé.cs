using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{

   public class PlanTexturé : PrimitiveDeBase
   {
      protected ScèneDeJeu ScèneJeu { get; set; }
      protected Vector3 Position { get; set; }
      protected Vector3 Rotation { get; set; }
      protected Vector3 Origine { get; set; }
      protected Vector2 Étendue { get; set; }
      protected Vector2 Dimensions { get; set; }
      protected float DeltaX { get; set; }
      protected float DeltaY { get; set; }
      Texture2D TexturePlan { get; set; }
      string NomTexturePlan { get; set; }
      protected Vector3[,] Points { get; set; }
      protected Vector2[,] PointsTexture { get; set; }
      protected VertexPositionTexture[] Sommets { get; set; }
      protected BasicEffect EffetDeBase { get; set; }
      int NbTrianglesParStrip { get; set; }
      int NbStrips { get; set; }



      public PlanTexturé(RPG jeu, ScèneDeJeu scèneJeu, Vector3 position, Vector3 rotation, Vector2 étendue, Vector2 dimensions, string nomtexture)
         : base(jeu)
      {
         ScèneJeu = scèneJeu;
         Position = position;
         Étendue = étendue;
         Dimensions = dimensions;
         Rotation = rotation;
         DeltaX = étendue.X / dimensions.X;
         DeltaY = étendue.Y / dimensions.Y;
         NomTexturePlan = nomtexture;
         Origine = new Vector3(-Étendue.X / 2, -Étendue.Y / 2, 0);
         Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
         Monde *= Matrix.CreateTranslation(Position);
      }

      public override void Initialize()
      {
         NbTrianglesParStrip = 2 * (int)Dimensions.X;
         NbTriangles = NbTrianglesParStrip * (int)Dimensions.Y;
         NbSommets = 2 * (int)(Dimensions.X + 1) * (int)Dimensions.Y;
         Points = new Vector3[(int)Dimensions.X + 1, (int)Dimensions.Y + 1]; //(il faut un sommet de plus que de carrés)
         //Points = new Vector3[(int)Dimensions.X, (int)Dimensions.Y];
         PointsTexture = new Vector2[(int)Dimensions.X + 1, (int)Dimensions.Y + 1]; //(il faut un sommet de plus que de carrés)
         //PointsTexture = new Vector2[(int)Dimensions.X, (int)Dimensions.Y];
         Sommets = new VertexPositionTexture[NbSommets];
         InitialiserPoints();
         base.Initialize();
      }

      protected override void LoadContent()
      {
         TexturePlan = ScèneJeu.GestionnaireDeTextures.Find(NomTexturePlan);
         EffetDeBase = EffetDeBase == null ? new BasicEffect(GraphicsDevice) : EffetDeBase;
         EffetDeBase.TextureEnabled = true;
         EffetDeBase.Texture = TexturePlan;
         base.LoadContent();
      }


      protected virtual void InitialiserPoints()
      {
         for (int j = 0; j < Dimensions.Y + 1; ++j)
         {
            for (int i = 0; i < Dimensions.X + 1; ++i)
            {
               Points[i, j] = new Vector3(Origine.X + DeltaX * i, Origine.Y + DeltaY * j, Origine.Z);
               PointsTexture[i, j] = new Vector2(i / Dimensions.X, 1 - (j / Dimensions.Y));
            }
         }
      }
      
      protected override void InitialiserSommets()
      {
         int iSommet = 0;
         for (int j = 0; j < Dimensions.Y; ++j)
         {
            for (int i = 0; i < Dimensions.X + 1; ++i)
            {
               Sommets[iSommet] = new VertexPositionTexture(Points[i, j], PointsTexture[i,j]);
               ++iSommet;
               Sommets[iSommet] = new VertexPositionTexture(Points[i, j + 1], PointsTexture[i, j+1]);
               ++iSommet;

            }
         }
      }

      public override void Draw(GameTime gameTime)
      {
         BlendState oldBlendState = GraphicsDevice.BlendState;
         BlendState newBlendState = BlendState.AlphaBlend;
         GraphicsDevice.BlendState = newBlendState;
         EffetDeBase.World = GetMonde();
         EffetDeBase.View = ScèneJeu.CaméraJeu.Vue;
         EffetDeBase.Projection = ScèneJeu.CaméraJeu.Projection;
         foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
         {
            passeEffet.Apply();
            for (int i = 0; i < (int)Dimensions.Y; ++i)
            {
               GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                                     (PrimitiveType.TriangleStrip, Sommets, (NbTrianglesParStrip + 2) * i, NbTrianglesParStrip);
            }
         }
         GraphicsDevice.BlendState = oldBlendState;
         base.Draw(gameTime);
      }
   }
}
