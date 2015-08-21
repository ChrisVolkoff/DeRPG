using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   public struct FogData
   {
      public FogData(Vector3 color, float start, float end)
      {
         Color = color;
         Start = start;
         End = end;
      }

      public Vector3 Color;
      public float Start;
      public float End;
   }

   public class Terrain : PlanTexturé
   {
      static Vector3 ROTATION_TERRAIN = new Vector3(-MathHelper.PiOver2,0,0);
      public float[,] HeightMap { get; private set; }
      public float HauteurMax { get; private set; }
      public float HauteurMin { get; private set; }
      int IndexDébut { get; set; }
      FogData Fog { get; set; }
      public string NomMusicSpécifique { get; private set; }


      public Terrain(RPG jeu, ScèneDeJeu scèneJeu, Vector3 position, Vector2 étendue, Vector2 dimensions, string nomtexture, FogData fog, float[,] heightMap, int indexDébut, string nomMusicSpécifique)
         : base(jeu, scèneJeu, position, ROTATION_TERRAIN, étendue, dimensions, nomtexture)
      {
         IndexDébut = indexDébut;
         HeightMap = heightMap;
         Fog = fog;
         NomMusicSpécifique = nomMusicSpécifique;
        
         Origine = new Vector3(0,0,0);
      }
      public override void Update(GameTime gameTime)
      {
         
         base.Update(gameTime);
      }
      protected override void LoadContent()
      {
         EffetDeBase = new BasicEffect(GraphicsDevice);
         EffetDeBase.FogEnabled = true;
         EffetDeBase.FogColor = Fog.Color;
         EffetDeBase.FogStart = Fog.Start; //30
         EffetDeBase.FogEnd = Fog.End; //450
         
         base.LoadContent();
      }

      protected override void InitialiserPoints()
      {
         for (int i = 0; i <= Dimensions.X; ++i) //(il faut un sommet de plus que de carrés)
         {
            for (int j = 0; j <= Dimensions.Y; ++j)
            {
               Points[i, j] = new Vector3(Origine.X + DeltaX * i, Origine.Y + DeltaY * j, HeightMap[i,IndexDébut+j]);
               PointsTexture[i, j] = new Vector2(i / (Dimensions.X), 1 - (j / (Dimensions.Y)));
            }
         }
      }

      public Vector2 GetRandomCoordinate()
      {
          Vector2 randCoord = new Vector2(Position.X + (float)Jeu.GénérateurAléatoire.Next(0, (int)Étendue.X + 1), Position.Z + (float)Jeu.GénérateurAléatoire.Next(0, (int)Étendue.Y + 1));
          return randCoord;
      }

   }
}
