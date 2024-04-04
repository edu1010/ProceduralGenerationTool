using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LSystem : MonoBehaviour
{
   public int numberOfCicles = 4;
   public string startRuleForTree = "X";//axiom in the book
   public List<(Vector3, Quaternion)> SavedPositions = new List<(Vector3, Quaternion)>();

   //Char is the key and the string the rule F implie go forward
   private Dictionary<char, string> rules;
   private String currentString = "";
   private GameObject parent;
   public TextMeshProUGUI cicles;

   public bool created = false;
   private void Start()
   {
      cicles.text = numberOfCicles+"";
      rules = new Dictionary<char, string>()
      {
         // //Rules from : beauty of  fractal trees
         // { 'X', "[FX][-FX][+FX]" },
         // //{ 'X', "[F-[[X]+X]+F[+FX]-X]" },
         // //{ 'X', "[F-[[X]+X]+F[+FX]-X]" },
         // { 'F', "[FF]" }
         { 'X',"[FX][-FX][+FX]"}, 
         {'F',"FF"}
         
      };
      CreateTree();
   }

  
   public void CreateTree()
   {
      created = true;
      if(parent!=null)
         Destroy(parent);
      parent = Instantiate(gameObject, transform);
      currentString = startRuleForTree;
      String currentStringCopy = currentString;
      for (int i = 0; i < numberOfCicles; i++)
      {
         foreach (char ch in currentString)
         {
            if (rules.ContainsKey(ch))
            {
               currentStringCopy += rules[ch];
            }
            else
            {
               currentStringCopy += ch;
            } 
         }   
         currentString = currentStringCopy;
         currentStringCopy = String.Empty;

      }
      //no aplico directamente al current pq afectaría al foreach
      print("current "+currentString);
      //Volvemos a recorrer y definimos las normas
      //ApplyRules(currentString);
      ApplyRules(currentString);
   }
   public GameObject branchToSpawn;
   public float sizeOfBranch = 1f;
   private float angle = 30f;
   public void ApplyRules(string currentStringApplyRules)
   {
      //Rules from wikipedia exaple 7 01/02/2023 https://en.wikipedia.org/wiki/L-system
      
      /*Here, F means "draw forward",
      − means "turn right 25°", 
      and + means "turn left 25°". 
      X does not correspond to any drawing action and is used to control the evolution of the curve.
      The square bracket "[" corresponds to saving the current values for position and angle,
      which are restored when the corresponding "]" is executed*/
 
      foreach (char ch in currentStringApplyRules)
      {
         String pos = "";
         foreach (var VARIABLE in SavedPositions)
         {
            pos += VARIABLE + " ";
         }
         print(pos);
         
         switch (ch)
         {
            case 'F':
               GameObject branchOfTree = Instantiate(branchToSpawn,parent.transform);
               //marcamos donde comienza la rama
               branchOfTree.GetComponent<LineRenderer>().SetPosition(0,transform.position);
               //movemos el transform y volvemos a aplicar la posicion al segundo punto del line. No se puede aplicar la suma unicamente al linerenderer, pq usaremso la posición del transform en las siguientes
               transform.Translate(Vector3.up * sizeOfBranch);
              // transform.position += transform.up * sizeOfBranch;
               branchOfTree.GetComponent<LineRenderer>().SetPosition(1,transform.position );
               break;
            case 'X':
               break;
            case '+':
               print("rot "+transform.rotation);
               transform.Rotate(Vector3.back * angle);
               print("rot "+transform.rotation);
               break;
            case '-':   
               print("rot "+transform.rotation);
               transform.Rotate(Vector3.forward * angle);
               print("rot "+transform.rotation);
               break;
            case '[':
               //Debemos guardar la posición y el angulo para poder volver a una posición anterior y crear otra bifurcación, sino solo crea lineas crecientes
               SavedPositions.Add((transform.position,transform.rotation));
               break;
            case ']':
               //Volvemos a esa posición anterior
               print(transform.position);
               transform.position = SavedPositions[^1].Item1;//rider me suguiere poner esto en vez de count-1
               transform.rotation = SavedPositions[^1].Item2;
               SavedPositions.RemoveAt(SavedPositions.Count-1);
               break;
            
         }
         
      }

   }
      public void IncrementIterations()
      {
         numberOfCicles += 1;
         
         cicles.text = numberOfCicles+"";
      }
      public void DecrementIterations()
      {
         numberOfCicles -= 1;
         if (numberOfCicles < 0)
         {
            numberOfCicles = 0;
            
         }
         
         cicles.text = numberOfCicles+"";
      }
}
