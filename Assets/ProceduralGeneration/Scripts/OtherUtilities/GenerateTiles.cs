using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GenerateTiles : MonoBehaviour
{
    [SerializeField] Texture2D input;

    [SerializeField] private int quadraticSize = 16;

    private int realQuadraticSize
    {
        get
        {
            return quadraticSize - 1;
        }
    }

    private int totalOfTiles;
    public List<Texture2D> _tiles;
    public List<Texture2D> _Repetedtiles;
    [SerializeField]
    public List<List<int>> posicionesDeCadaTile = new List<List<int>>();
    void Start()
    {
        GenerateListOfTiles();
    }

    public void GenerateListOfTiles()
    {
        //DEVUELVE DE DERECHA A IZQUIRDA. Y LA PRIMERA FILA SERA LA ULTIMA EN GENERAR. PIMER ELEMENTO ULTIMO
        int sourceMipLevel = 0; //0 es resolucion original
        Color32[] pixels = input.GetPixels32(sourceMipLevel);
        totalOfTiles = pixels.Length / (quadraticSize*quadraticSize);
        Color32[,] pixelsToArray = new Color32[input.width,input.height];
        int l_x = 0;
        int l_y = 0;
        //pixelsToArray[l_x, l_y] = pixels[0];
        /*for (int i = 1; i < pixels.Length-1; i++)
        {
            l_x++;
            if (l_x >= input.width)
            {
                l_y ++;
                l_x= 0;
            }
            if(l_y> input.height)
            {
                return;
            }
            pixelsToArray[l_x,l_y] = pixels[i];

        }*/
        for (int y = 0; y < input.height; y++) {
            for (int x = 0; x < input.width; x++) {
                pixelsToArray[x, y] = pixels[y * input.width + x];
            }
        }

        int numTilesPerRow = input.width / quadraticSize;
        int numTilesPerCol = input.height / quadraticSize;
        
        
        Color32[] tilePixels = new Color32[quadraticSize * quadraticSize];

                
                
        int numMatricesX = numTilesPerRow; // Número de matrices internas a lo largo del eje x
        int numMatricesY = numTilesPerCol; // Número de matrices internas a lo largo del eje y

        int matrixWidth = input.width / numMatricesX; // Ancho de cada matriz interna
        int matrixHeight = input.height / numMatricesY; // Altura de cada matriz interna

        for (int matrixY = 0; matrixY < numMatricesY; matrixY++) {
            for (int matrixX = 0; matrixX < numMatricesX; matrixX++) {
                // Coordenadas de la esquina superior izquierda de la matriz interna actual
                int startX = matrixX * matrixWidth;
                int startY = matrixY * matrixHeight;

                int increment = 0;
                for (int y = startY; y < startY + matrixHeight; y++) {
                    for (int x = startX; x < startX + matrixWidth; x++) {
                        tilePixels[increment] = pixelsToArray[x, y];
                        increment++;
                    }
                }
                Texture2D tile = new Texture2D(quadraticSize, quadraticSize);
                tile.SetPixels32(tilePixels);
                tile.Apply();

                _tiles.Add(tile);
                SaveImage(tile,_tiles.Count);
            }
        }
       


        for (int filaTileY = 0; filaTileY < numTilesPerCol; filaTileY++) {
            for (int columTileX = 0; columTileX < numTilesPerRow; columTileX++) {
                
                // Extract tile from pixelsToArray
              
                
                
                
                
                
                /*int startX = x * quadraticSize;
                int startY = y * quadraticSize;
                if (x != 0)
                {
                    startX -= realQuadraticSize;
                    startY-= realQuadraticSize;
                }
                for (int localColorY = 0; localColorY < quadraticSize; localColorY++) 
                {
                    for (int localColorX = 0; localColorX < quadraticSize; localColorX++) 
                    {
                        tilePixels[localColorY*quadraticSize+localColorX] =pixels[localColorY*input.width+localColorX];
                    }
                }*/
                // Create Texture2D for tile
               
            }
        }
        
        
        
        
        //quadraticSize--;
        /*for (int i = 1; i < totalOfTiles; i++)
        {
            Texture2D tile = new Texture2D(quadraticSize, quadraticSize);
            for (int x = 0; x < realQuadraticSize; x++)
            {
                for (int y = 0; y < realQuadraticSize; y++)
                {
                    //tile.SetPixel(x, y, pixelsToArray[x*i,y*i]);
                    int tmpX = (i * realQuadraticSize) + x;
                    int tmpY = (i * realQuadraticSize) + y;
                    tile.SetPixel(x, y, pixelsToArray[tmpX, tmpY]);

                    //borrar

                }
            }
            tile.Apply();
            _tiles.Add(tile);
            SaveImage(tile,1);
            
        }*/
        CalculateRepitedTiles();
    }

    public void CalculateRepitedTiles()
    {
        List<int> positionsAlredyFound = new List<int>();
        
        int currentIndex = 0;
        foreach (var tile in _tiles)
        {
            if (!positionsAlredyFound.Contains(currentIndex))
            {
                List<int> positionsOfCurrentTile = CompareTextureS(tile, _tiles);
                positionsAlredyFound.AddRange(positionsOfCurrentTile);
                posicionesDeCadaTile.Add(positionsOfCurrentTile) ;
                _Repetedtiles.Add(tile);
            }
            currentIndex++;
        }
    }

    public void SaveImage(Texture2D texture,int id)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "Image" +id+ ".png", bytes);
    }

    private bool first = true;


    public int CalcularPixelIndex(int numeroDeTileEnquEstoy,int tileX,int tileY,Color32[]pixels )
    {
        int totalOfColors = pixels.Length;
        //int pixelIndex = (quadraticSize * quadraticSize) - (tileX * tileY) * numeroDeTileEnquEstoy;
        //return pixelIndex;
       // int pixelIndex = (quadraticSize * quadraticSize) - ((tileX * quadraticSize) + tileY) * numeroDeTileEnquEstoy;
       //posicion = (numeroDeCuadrado * tamanoCuadradoInterno * tamanoCuadradoInterno) + (posicionY * tamanoCuadradoInterno) + posicionX;
       //posicion = (numeroDeCuadrado * tamanoCuadradoInterno * tamanoCuadradoInterno) + (posicionY * tamanoCuadradoInterno) + posicionX;

    

       int pixelIndex = ((numeroDeTileEnquEstoy)* quadraticSize * quadraticSize) + (tileY * quadraticSize) + tileX;
       if (pixelIndex >= totalOfColors)
       {
           if (first)
           {
               first = false;
               Debug.Log(totalOfColors + " " + pixelIndex); 
               Debug.Log("numeroDeTileEnquEstoy:"+numeroDeTileEnquEstoy +" x: "+tileX+" y:"+tileY+" pixelIndex: " + pixelIndex); 

           }
      
       return 0;
       }
       else
       {
        return pixelIndex;
       }
    }
    
    //code found in https://stackoverflow.com/questions/51555468/how-to-compare-two-sprite-textures-in-unity
    private bool CompareTexture (Texture2D first, Texture2D second)
    {
        Color[] firstPix = first.GetPixels();
        Color[] secondPix = second.GetPixels();
        if (firstPix.Length!= secondPix.Length)
        {
            return false;
        }
        for (int i= 0;i < firstPix.Length;i++)
        {
            if (firstPix[i] != secondPix[i])
            {
                return false;
            }
        }

        return true;
    }
    private List<int> CompareTextureS (Texture2D first,List<Texture2D> textureArray)
    {
        List<int> positions = new List<int>();
        int i = 0;
        foreach (var texture in textureArray)
        {
            if (CompareTexture(first, texture))
            {
                positions.Add(i);
            }
            i++;
        }
Debug.Log(string.Join(",", positions));
        return positions;
    }
}

