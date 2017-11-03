using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	public GameObject spawnPoint;

	[SerializeField]
	private GameObject[] menuItems;
	[SerializeField]
	private int[] menuItemCounts;
	public bool empty = false;

	public bool Empty {
		get {
			if (empty) {
				return empty;
			}

			empty = true;
			for (int i = 1; i < menuItemCounts.Length; i++) {
				if (menuItemCounts [i] != 0) {
					empty = false;
					break;
				}
			}
			return empty;
		}
	}

	private int currentMenuItem;
	void Start () {
		foreach (GameObject menuItem in menuItems) {
			menuItem.SetActive (false);
		}
		currentMenuItem = 1;
	}

	public void EnableMenu () {
		Debug.Log ("Enable Menu");
		//  See if there are any objects left

		if (Empty) {
			//  show empty text
			currentMenuItem = 0;
		}

		if (menuItemCounts [currentMenuItem] == 0) {
			NextMenuItem ();
		}

		if (Empty) {
			currentMenuItem = 0;
		}

		menuItems [currentMenuItem].SetActive (true);
	}

	public void DisableMenu () {
		menuItems [currentMenuItem].SetActive (false);
	}

	int TryNextNonEmptyMenuItem () {
		int tempCurrentMenuItem = currentMenuItem;
		do {
			tempCurrentMenuItem += 1;
			if (tempCurrentMenuItem > menuItems.Length - 1) {
				tempCurrentMenuItem = 1;
			}
			if (tempCurrentMenuItem == currentMenuItem) {
				return currentMenuItem;
			}
			if (menuItemCounts[tempCurrentMenuItem] != 0) {
				return tempCurrentMenuItem;
			}
		} while (true);
	}
	int TryPrevNonEmptyMenuItem () {
		int tempCurrentMenuItem = currentMenuItem;
		do {
			tempCurrentMenuItem -= 1;
			if (tempCurrentMenuItem < 1) {
				tempCurrentMenuItem = menuItems.Length - 1;
			}
			if (tempCurrentMenuItem == currentMenuItem) {
				return currentMenuItem;
			}
			if (menuItemCounts[tempCurrentMenuItem] != 0) {
				return tempCurrentMenuItem;
			}
		} while (true);
	}

	public void NextMenuItem () {
		Debug.Log ("NextMenuItem");
		menuItems [currentMenuItem].SetActive (false);
		if (Empty) {
			currentMenuItem = 0;
		} else {
			int nextMenuItem = TryNextNonEmptyMenuItem ();
			if (menuItemCounts [nextMenuItem] == 0) {
				empty = true;
				currentMenuItem = 0;
			} else {
				currentMenuItem = nextMenuItem;
			}
			/*
			int tempCurrentMenuItem = currentMenuItem;
			while (menuItemCounts [tempCurrentMenuItem] == 0) {
				Debug.Log ("Looking");
				tempCurrentMenuItem += 1;

				if (tempCurrentMenuItem > menuItems.Length - 1) {
					tempCurrentMenuItem = 1;
				}

				if (tempCurrentMenuItem == currentMenuItem) {
					empty = true;
					currentMenuItem = 0;
					break;
				}
				currentMenuItem = tempCurrentMenuItem;
			}
			*/
		}
		menuItems [currentMenuItem].SetActive (true);
	}

	public void PrevMenuItem () {
		Debug.Log ("PrevMenuItem");
		menuItems [currentMenuItem].SetActive (false);
		if (Empty) {
			currentMenuItem = 0;
		} else {
			int prevMenuItem = TryPrevNonEmptyMenuItem ();
			if (menuItemCounts [prevMenuItem] == 0) {
				empty = true;
				currentMenuItem = 0;
			} else {
				currentMenuItem = prevMenuItem;
			}
			/*
			int tempCurrentMenuItem = currentMenuItem;
			while (menuItemCounts [tempCurrentMenuItem] == 0) {
				tempCurrentMenuItem -= 1;
				if (tempCurrentMenuItem < 1) {	
					tempCurrentMenuItem = menuItems.Length - 1;
				}
				if (tempCurrentMenuItem == currentMenuItem) {
					empty = true;
					currentMenuItem = 0;
					break;
				}
				currentMenuItem = tempCurrentMenuItem;
			}
			*/
		}
		menuItems [currentMenuItem].SetActive (true);
	}

	public GameObject GetMenuItemPrefab () {
		if (Empty) {
			return null;
		}
		int tempMenuItem = currentMenuItem;

		menuItemCounts [currentMenuItem] -= 1;
		if (menuItemCounts [currentMenuItem] == 0) {
			NextMenuItem ();
		}

		return menuItems [tempMenuItem].GetComponent<ObjectMenuItem> ().MyPrefab;
	}
}
