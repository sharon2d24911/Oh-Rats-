using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragCombination : MonoBehaviour
{
    private GameObject selectedObject;
    private GameObject baseObject;
    private Vector2 startingPosition;
    [HideInInspector] public List<GameObject> combining = new List<GameObject>();
    [HideInInspector] public List<GameObject> dragged = new List<GameObject>();
    public Dictionary<Vector2, GameObject> filledPositions = new Dictionary<Vector2, GameObject>();
    private Dictionary<string, (bool, GameObject)> unlockedDonuts = new Dictionary<string, (bool,GameObject)>();
    public GameObject combinationZone;
    private readonly float sensitivity = 3.0f;
    private bool isIngredient;
    private GameObject grid;
    public GameObject unit;
    public GameObject content;
    public GameObject notification;
    public Button mixButton;
    public Texture2D garbageCursor;
    public Texture2D defaultCursor;
    private GameObject newUnit;
    public int newDonutsNum = 0; //number that is displayed for the notification
    private string sugarDropSound;
    private string flourDropSound;
    private string eggDropSound;
    private string sugarGrabSound;
    private string flourGrabSound;
    private string eggGrabSound;
    private string donutGrabSound;
    private int sugarDropCount = 1;
    private int flourDropCount = 1;
    private int eggDropCount = 1;
    private bool mixButtonClickedEgg = false;
    private bool mixButtonClickedFlour = false;
    private bool mixButtonClickedSugar = false;
    [HideInInspector] public GameObject[] allIngredients;
    [HideInInspector] public bool trashMode;
    [HideInInspector] public bool tutorialMode;
    [HideInInspector] public Vector2 topLeft;
    private bool bowlFull = false;
    private int z = 0;

    //=====Animation stuff=======
    public float frameRate = 4f;
    private float animTimer;
    public float animTimeMax; //max seconds per frame. concept taken from lab 4
    private int animIndex = 0;
    public Sprite bowlDefault;
    public List<Sprite> bowlAnims = new List<Sprite>();
    private bool bowlIsAnimating = false;
    //=====Animation stuff=======


    void Start()
    {
        grid = GameObject.Find("Grid");
        allIngredients = GameObject.FindGameObjectsWithTag("Ingredient");
        animTimeMax = animTimeMax / frameRate;
        trashMode = false;

        if (content)
        {
            for (int i = 1; i < 4; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    string combo1 = string.Format("{0}:{1}:1", i, j);
                    GameObject donut1 = content.transform.Find(combo1).gameObject;
                    unlockedDonuts.Add(combo1, (false, donut1));
                    string combo2 = string.Format("{0}:{1}:2", i, j);
                    GameObject donut2 = content.transform.Find(combo2).gameObject;
                    unlockedDonuts.Add(combo2, (false, donut2));
                    string combo3 = string.Format("{0}:{1}:3", i, j);
                    GameObject donut3 = content.transform.Find(combo3).gameObject;
                    unlockedDonuts.Add(combo3, (false, donut3));
                }
            }
        }
        

        tutorialMode = false;
        topLeft = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the mix button should allow interaction
        if (!CheckMinimum())
            mixButton.interactable = false;
        else
            mixButton.interactable = true;

        // When left mouse is pressed
        if (Input.GetMouseButtonDown(0) && Time.timeScale != 0f)
            CheckHitObject();

        // If left mouse button is held down
        if (Input.GetMouseButton(0) && selectedObject != null)
            DragObject();

        // If left mouse button is released
        if (Input.GetMouseButtonUp(0) && selectedObject != null)
            DropObject();

        if (bowlIsAnimating)
        {
            bowlAnimate();
        }

    }

    void bowlAnimate()
    {
        int animFrames = bowlAnims.Count;
        animTimer += Time.deltaTime;

        if (animTimer > animTimeMax)
        {
            animTimer = 0;
            if (animIndex < animFrames - 1)
            {
                animIndex++;
            }
            else
            {
                animIndex = 0;
            }

            combinationZone.GetComponent<SpriteRenderer>().sprite = bowlAnims[animIndex];
        }
    }

    
    // Checks if there is an object that can be selected at the mouse position
    void CheckHitObject()
    {
        // Gets mouse coordinates and maps to where it is on the game screen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Raycasting: did the mouse hit a collider?
        RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (hit.collider != null)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
            selectedObject = hit.collider.gameObject;
            startingPosition = selectedObject.transform.position;

            if (!dragged.Contains(selectedObject) && selectedObject.tag != "Prop" && (selectedObject.tag == "Ingredient" || selectedObject.tag == "Unit"))
            {
                // Separates behaviour depending on selected object type
                if (selectedObject.tag == "Ingredient" && selectedObject.GetComponent<Ingredient>().remaining > 0)
                {
                    isIngredient = true;
                    GameObject ingredient = Instantiate(selectedObject.GetComponent<Ingredient>().singularIngredient);

                    // Sfx for ingredient grab
                    if (selectedObject.name == "Sugar") // sugar sfx
                    {
                        string[] sugarGrabSound = { "SugarGrab1", "SugarGrab2" };
                        this.sugarGrabSound = sugarGrabSound[Mathf.FloorToInt(Random.Range(0, 2))];
                        AudioManager.Instance.PlaySFX(this.sugarGrabSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.sugarGrabSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.sugarGrabSound][1]);
                    }
                    else if (selectedObject.name == "Flour") // flour sfx
                    {
                        string[] flourGrabSound = { "FlourGrab1", "FlourGrab2" };
                        this.flourGrabSound = flourGrabSound[Mathf.FloorToInt(Random.Range(0, 2))];
                        AudioManager.Instance.PlaySFX(this.flourGrabSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.flourGrabSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.flourGrabSound][1]);
                    }
                    else if (selectedObject.name == "Egg") // egg sfx
                    {
                        string[] eggGrabSound = { "EggGrab1", "EggGrab2", "EggGrab3" };
                        this.eggGrabSound = eggGrabSound[Mathf.FloorToInt(Random.Range(0, 3))];
                        AudioManager.Instance.PlaySFX(this.eggGrabSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.eggGrabSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.eggGrabSound][1]);
                    }

                    baseObject = selectedObject;
                    selectedObject = ingredient;
                    // Set parent to be the ingredient
                    selectedObject.transform.SetParent(baseObject.transform, true);
                }
                else if (selectedObject.tag == "Unit")
                {
                    isIngredient = false;
                    string[] donutGrabSound = { "DonutGrab1", "DonutGrab2" };
                    this.donutGrabSound = donutGrabSound[Mathf.FloorToInt(Random.Range(0, 2))];
                    AudioManager.Instance.PlaySFX(this.donutGrabSound, GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.donutGrabSound][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary[this.donutGrabSound][1]);
                }
                else
                    selectedObject = null;
            } else if (selectedObject.tag == "Unit" && trashMode)
            {
                // If unit is clicked on and you are in trash mode
                Debug.Log("TRASH MODE: " + selectedObject + " goodbye.");
                // Destroy unit and clear up position
                filledPositions.Remove(selectedObject.transform.position);
                AudioManager.Instance.PlaySFX("UnitDiscard", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UnitDiscard"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["UnitDiscard"][1]);
                Destroy(selectedObject);
                trashMode = false;
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
            } else // Nothing else can be dragged, but add condition for trash if needed
                selectedObject = null;

            
        }
    }

    // Drags selected object if something is at mouse position
    void DragObject()
    {
        // Set transparency of object to 70%
        selectedObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .7f);
        // Change the position of the selected object based on the position of the mouse (accounting for offset)
        selectedObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane + 10.0f));
    }

    // Drops object that was being dragged by mouse
    void DropObject()
    {
        // Resets object transparency
        selectedObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        Vector2 selectedV2 = selectedObject.transform.position;

        GridCreate gridScript = grid.GetComponent<GridCreate>();
        Vector2 nearestPos = startingPosition;
        float nearestDistance = Vector2.Distance(grid.transform.position, selectedV2);
        int gridDepth = 0;
        List<Vector3> gridPositions;
        gridPositions = gridScript.getPositions(); //grabs list of grid positions from the GridCreate script

        foreach (Vector2 p in gridPositions)
        {
            float newDistance = Vector2.Distance(p, selectedV2);
            if (newDistance < nearestDistance)
            {
                nearestDistance = newDistance;
                gridDepth = (gridPositions.IndexOf(p) / gridScript.columns);
                //Debug.Log("gridDepth " + gridDepth);
                nearestPos = p;
            }
        }

        // Use for tutorial only, forces player to put donut only on the top left grid spot
        if (tutorialMode)
        {
            topLeft = gridPositions[0];
            if (filledPositions.Count == 0)
            {
                for (int j = 1; j < gridPositions.Count; j++)
                {
                    Debug.Log("ADDING TO: " + gridPositions[j]);
                    filledPositions.Add(gridPositions[j], selectedObject);
                }
            }
            
        }

        if (isIngredient)
        {
            Vector2 combV2 = combinationZone.transform.position;

            // Checks if the selected object is close enough or on the object that's been designated as the combination area
            if (Vector2.Distance(selectedV2, combV2) < sensitivity && !bowlFull)
            {
                // Check if there are >3 of the ingredient in the combining list
                if (selectedObject.transform.parent.transform.childCount > 3)
                {
                    Debug.Log("Can't have more than three of one ingredient!");
                    Destroy(selectedObject);
                    return;
                }

                // Snaps object into the same position
                Vector3 offset = new Vector3(Random.Range(-.9f, .9f), Random.Range(-.2f, .3f));
                selectedObject.GetComponent<SpriteRenderer>().sortingOrder = z;
                z += 1;
                selectedObject.transform.position = combinationZone.transform.position + offset;
                if (selectedObject.transform.childCount > 0)
                {
                    selectedObject.GetComponent<SpriteRenderer>().sprite = selectedObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                }
                dragged.Add(selectedObject);

                // Sfx for ingredient drop
                if (selectedObject.name == "Sugar_individual(Clone)") // sugar sfx
                {
                    // Check if mix button is clicked, if so then play the first sfx
                    if (mixButtonClickedSugar)
                    {
                        sugarDropCount = 1;
                        mixButtonClickedSugar = false;
                    }
                    if (sugarDropCount == 1)
                    {
                        AudioManager.Instance.PlaySFX("SugarDrop1", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarDrop1"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarDrop1"][1]);
                        sugarDropCount++;
                    }
                    else if (sugarDropCount == 2)
                    {
                        AudioManager.Instance.PlaySFX("SugarDrop2", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarDrop2"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarDrop2"][1]);
                        sugarDropCount++;
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX("SugarDropMax", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarDropMax"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarDropMax"][1]);
                        sugarDropCount = 1;
                    }
                }
                else if (selectedObject.name == "Flour_individual(Clone)") // flour sfx
                {
                    // Check if mix button is clicked, if so then play the first sfx
                    if (mixButtonClickedFlour)
                    {
                        flourDropCount = 1;
                        mixButtonClickedFlour = false;
                    }
                    if (flourDropCount == 1)
                    {
                        AudioManager.Instance.PlaySFX("FlourDrop1", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourDrop1"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourDrop1"][1]);
                        flourDropCount++;
                    }
                    else if (flourDropCount == 2)
                    {
                        AudioManager.Instance.PlaySFX("FlourDrop2", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourDrop2"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourDrop2"][1]);
                        flourDropCount++;
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX("FlourDropMax", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourDropMax"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourDropMax"][1]);
                        flourDropCount = 1;
                    }
                }
                else if (selectedObject.name == "Egg_individual(Clone)") // egg sfx
                {
                    // Check if mix button is clicked, if so then play the first sfx
                    if (mixButtonClickedEgg)
                    {
                        eggDropCount = 1;
                        mixButtonClickedEgg = false;
                    }
                    if (eggDropCount == 1)
                    {
                        AudioManager.Instance.PlaySFX("EggDrop1", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggDrop1"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggDrop1"][1]);
                        eggDropCount++;
                    }
                    else if (eggDropCount == 2)
                    {
                        AudioManager.Instance.PlaySFX("EggDrop2", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggDrop2"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggDrop2"][1]);
                        eggDropCount++;
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX("EggDropMax", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggDropMax"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggDropMax"][1]);
                        eggDropCount = 1;
                    }
                }

                // Adds selected object to list
                combining.Add(selectedObject);

                // Once placed, decrease count for that ingredient by 1
                baseObject.GetComponent<Ingredient>().UseIngredient();

            }
            else // Destroy the ingredient instance selected if not placed close enough
            {
                if (selectedObject.name == "Egg_individual(Clone)")
                {
                    AudioManager.Instance.PlaySFX("EggReturn", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggReturn"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["EggReturn"][1]);
                }
                else if (selectedObject.name == "Flour_individual(Clone)")
                {
                    AudioManager.Instance.PlaySFX("FlourReturn", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourReturn"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["FlourReturn"][1]);
                }
                else if (selectedObject.name == "Sugar_individual(Clone)")
                {
                    AudioManager.Instance.PlaySFX("SugarReturn", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarReturn"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["SugarReturn"][1]);
                }
                Destroy(selectedObject);
            }
        }
        else if (nearestDistance > (sensitivity) || nearestPos == selectedV2 || filledPositions.ContainsKey(nearestPos))
        {
            // If Unit is not within distance, place back in original spot
            selectedObject.transform.position = new Vector3(startingPosition.x, startingPosition.y, 1);
            AudioManager.Instance.PlaySFX("DonutReturn", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DonutReturn"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DonutReturn"][1]);
        }
        else
        {
            // If tower is within distance of a grid spot, snaps object into the same position
            selectedObject.transform.position = new Vector3(nearestPos.x, nearestPos.y, 5f - gridDepth);
            selectedObject.GetComponent<UnitBehaviour>().placed = true;
            selectedObject.GetComponent<UnitBehaviour>().layerSprites(6*gridDepth + 1);
            filledPositions.Add(nearestPos, selectedObject); //puts unit in dictionary, position will no longer be free on the grid
            dragged.Add(selectedObject);
            AudioManager.Instance.PlaySFX("DonutPlace", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DonutPlace"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DonutPlace"][1]);
            bowlFull = false;
        }

        selectedObject = null;
    }

    public void CheckIfCombine()
    {
        // If player hits mix button, use up all the placed ingredients by tallying up their stats
        float attack = 0;
        float speed = 0;
        float health = 0;

        mixButtonClickedEgg = true;
        mixButtonClickedFlour = true;
        mixButtonClickedSugar = true;
        // Clear ingredients used counters
        foreach (GameObject ingredient in allIngredients)
        {
            ingredient.GetComponent<Ingredient>().ClearUse();
        }

        // Pull each item out and add the stats up, then instantiate a unit with those stats & correct layering
        while (combining.Count > 0)
        {
            dragged.Remove(combining[0]);
            attack += combining[0].GetComponentInParent<Ingredient>().attack;
            speed += combining[0].GetComponentInParent<Ingredient>().speed;
            health += combining[0].GetComponentInParent<Ingredient>().health;
            Destroy(combining[0]);
            combining.Remove(combining[0]);
        }
        Debug.Log("BOOSTING:\nAttack: " + attack + ", Speed: " + speed + ", Health: " + health);
        z = 0;
        // Create tower with those stats
        StartCoroutine(CombineWithDelay(attack, speed, health));

    }

    // Checks if the user has placed at minimum one of each ingredient
    public bool CheckMinimum()
    {
        // Minimum of 3 ingredients, early check
        if (combining.Count < 3)
            return false;
        
        // Checks if each ingredient has at least one child (and is not the currently selected one, as that hasn't been placed in the bowl yet)
        foreach (GameObject ingredient in allIngredients)
        {
            if (ingredient.transform.childCount < 1 || (ingredient.transform.childCount == 1 && selectedObject != null && selectedObject.transform.parent == ingredient.transform))
            {
                return false;
            }
        }
        return true;
        
    }

    // Creates new unit based on the given stats
    IEnumerator CombineWithDelay(float addAttack, float addSpeed, float addHealth)
    {
       //Begin bowl animation
        bowlIsAnimating = true;

        bowlFull = true;
        AudioManager.Instance.PlaySFX("Mixing", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["Mixing"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["Mixing"][1]);
        // Wait for 3 seconds
        yield return new WaitForSeconds(3);

        //End bowl animation
        bowlIsAnimating = false;
        combinationZone.GetComponent<SpriteRenderer>().sprite = bowlDefault;

        newUnit = Instantiate(unit, combinationZone.transform.position, Quaternion.identity);
        //Layer = Instantiate(layer1, combinationZone.transform.position, Quaternion.identity);

        // Change the unit's attack, speed, and health based on additional ingredient stats
        newUnit.GetComponent<UnitBehaviour>().projAddAttack += addAttack;
        newUnit.GetComponent<UnitBehaviour>().projAddSpeed += addSpeed;
        newUnit.GetComponent<UnitBehaviour>().health += addHealth;

        float attack = 0;
        float speed = 0;
        float health = 0;

        foreach (GameObject ingredient in allIngredients)
        {
            attack += ingredient.GetComponent<Ingredient>().attack;
            speed += ingredient.GetComponent<Ingredient>().speed;
            health += ingredient.GetComponent<Ingredient>().health;
        }

        int attackBoost = (int)(addAttack / attack) - 1;
        int speedBoost = (int)(addSpeed / speed) - 1;
        int healthBoost = (int)(addHealth / health) - 1;

        if (!tutorialMode)
        {
            string currentCombo = string.Format("{0}:{1}:{2}", (speedBoost + 1), (attackBoost + 1), (healthBoost + 1));
            (bool, GameObject) tuple;
            tuple = unlockedDonuts[currentCombo];
            if (!tuple.Item1 && !tutorialMode)
            {
                Debug.Log("new make");
                newDonutsNum += 1;
                unlockedDonuts[currentCombo] = (true, tuple.Item2);
                tuple.Item2.GetComponent<Image>().color += new Color(255, 255, 255, 0);
                notification.SetActive(true);
                notification.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newDonutsNum.ToString();

            }
            else
            {
                Debug.Log("already made");
            }


        }
        newUnit.GetComponent<UnitBehaviour>().attackBoost = attackBoost;
        newUnit.GetComponent<UnitBehaviour>().speedBoost = speedBoost;
        newUnit.GetComponent<UnitBehaviour>().healthBoost = healthBoost;
        newUnit.tag = "Unit";
    }

    

    // Unit removal
    public void RemoveUnit()
    {
        // Every time button is clicked it reverses the mode
        trashMode = !trashMode;

        if (trashMode)
        {
            Cursor.SetCursor(garbageCursor, Vector2.zero, CursorMode.ForceSoftware);
            AudioManager.Instance.PlaySFX("DiscardOn", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DiscardOn"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DiscardOn"][1]);
        } else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            AudioManager.Instance.PlaySFX("DiscardOff", GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DiscardOff"][0], GameObject.FindWithTag("GameHandler").GetComponent<ReadSfxFile>().sfxDictionary["DiscardOff"][1]);
        }
    }


    // if player wants to clear combination before mixing
    public void ClearBowl()
    {
        // Empty any objects on the combination zone
        if (combining.Count > 0)
        {
            // Clear ingredients used counters
            foreach (GameObject ingredient in allIngredients)
            {
                ingredient.GetComponent<Ingredient>().ClearUse();
            }

            foreach (GameObject item in combining)
            {
                item.GetComponentInParent<Ingredient>().AddIngredient(1);
                Destroy(item);
            }
        }
        // Clear anything that was already added to the list to be combined
        combining.Clear();
        foreach (GameObject obj in dragged.ToArray())
        {
            if (obj == null || obj.tag != "Unit")
            {
                dragged.Remove(obj);
            }

        }
    }
}