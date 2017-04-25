using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

   public GameObject BottomRight;
   public int RowCount;
   public int ColumnCount;
   public GameObject EnemyPrefab;
   public GameObject NetworkManager;

   private Dictionary<int, EnemyBehaviour> _enemies;
   private NetworkHandler _handler;

   void Start()
   {
      _enemies = new Dictionary<int, EnemyBehaviour>();
      _handler = NetworkManager.GetComponent<NetworkHandler>();
      if (_handler != null)
      {
         _handler.Register("marker_update", OnMove);
         print("Registered for network updates");
      }
      else
      {
         print("failed to register on network manager");
      }
   }

   private void OnMove(string message)
   {
      MarkerUpdate marker = JsonUtility.FromJson<MarkerUpdate>(message);
      print("Moving " + marker.Id + " to " + marker.Row + " " + marker.Column);
      Vector3 newPosition = GetAnchor(marker.Column, marker.Row);
      if (!_enemies.ContainsKey(marker.Id))
      {
         GameObject newEnemy = Instantiate(EnemyPrefab, newPosition, Quaternion.identity);
         newEnemy.GetComponent<EnemyBehaviour>().Id = marker.Id;
         _enemies.Add(marker.Id, newEnemy.GetComponent<EnemyBehaviour>());
      }
      else
      {
         _enemies[marker.Id].Move(newPosition);
      }
   }

   private Vector3 GetAnchor(int column, int row)
   {
      float xPos = Map(column, 0, ColumnCount, transform.position.x, BottomRight.transform.position.x);
      float yPos = Map(row, 0, RowCount, transform.position.y, BottomRight.transform.position.y);
      return new Vector3(xPos, yPos);
   }

   public Vector2 CalculateRowCol(Vector3 position)
   {
      int row = (int) Math.Round(Map(position.x, transform.position.x, BottomRight.transform.position.x, 0, ColumnCount));
      int column = (int) Math.Round(Map(position.y, transform.position.y, BottomRight.transform.position.y, 0, RowCount));
      return new Vector2(row, column);
   }

   private static float Map(float value, float inMin, float inMax, float outMin, float outMax)
   {
      return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
   }
}

[Serializable]
class MarkerUpdate
{
   public string Type;
   public int Id;
   public int Column;
   public int Row;
}
