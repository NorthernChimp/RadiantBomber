using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplayCrosshairScript : MonoBehaviour
{
	// Start is called before the first frame update
	public Transform dot;
	public Transform circle;
	public List<Transform> arrows;
	public bool drawDirection = false;
	public bool isVisible = false;
	public bool drawCircle;
	public bool drawArrow;
	public bool drawOnDoubleClickOnly = false;
	public bool followsPlayer;
	public float width = 2f;
	public List<SpriteRenderer> allSprites;
	public GameObject dotPrefab;
	public Transform thisTransform;
	public void DisplayCrosshair(float width, bool doWeDrawDirection,bool playerFollows, Transform theTransform)
	{
		followsPlayer = playerFollows;
		thisTransform = theTransform;
		width = width * MainScript.blockHeight * 0.25f;
		///circle.localScale = new Vector3(width / 2f, width / 2f, circle.localScale.z);
		drawDirection = doWeDrawDirection;
		List<SpriteRenderer> tempList = new List<SpriteRenderer>();
		//tempList.Add(dot.GetComponent<SpriteRenderer>());
		//tempList.Add(circle.GetComponent<SpriteRenderer>());
		if (drawDirection)
		{
			foreach (Transform t in arrows) { tempList.Add(t.GetComponent<SpriteRenderer>()); }
		}
	}
	public void SetupCrosshair(bool drawTheCircle,bool drawTheArrow,bool doubleClickOnly)
	{
		allSprites = new List<SpriteRenderer>();
		transform.parent = MainScript.thePlayer;
		drawOnDoubleClickOnly = doubleClickOnly;
		if(drawTheCircle) {DrawCircle(); drawCircle = true;}
		if(drawTheArrow) {DrawArrow(); drawArrow = true;}
		dot = Instantiate(dotPrefab, transform.position, Quaternion.identity).transform;
		dot.parent = transform;
		allSprites.Add(dot.GetComponent<SpriteRenderer>());
		float widthScale = (Screen.width / 20f) * 0.01f;
		dot.localScale = new Vector3(widthScale/0.16f, widthScale/0.16f, 1f);
        //dot.parent = transform;
        if (doubleClickOnly) { ChangeVisibility(false); }
	}
	public void ChangeVisibility(bool temp)
	{
		foreach(SpriteRenderer render in allSprites)
		{
			render.enabled = temp;
		}
	}
	public void DrawArrow()
	{
		arrows = new List<Transform>();
		for(int i = 0; i < 4;i++)
		{
			float widthScale = MainScript.blockWidth / 0.16f;
			Transform temp = Instantiate(dotPrefab, transform.position, Quaternion.identity).transform;
			arrows.Add(temp);
			temp.localScale = new Vector3(widthScale,widthScale,1f);
			allSprites.Add(temp.GetComponent<SpriteRenderer>());
			arrows[arrows.Count - 1].parent = transform;
		}
	}
	public void UpdateCrosshair(TouchInterface t)
	{
		Vector2 difference = t.GetRawDifference();
		Touch currentTouch = t.GetTouch();
		Vector2 sinceOriginDifference = currentTouch.position - t.origin;
		Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y + (Screen.height * 0.15f), 1f));
		//Vector3 halfwayBetweenOriginAndCurrent = new Vector3(t.origin.x + (sinceOriginDifference.x * 0.5f),t.origin.y + (sinceOriginDifference.y * 0.5f) + (Screen.height * 0.2f),1f);
		//Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(halfwayBetweenOriginAndCurrent);
		//Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y + (Screen.height * 0.15f), 1f));
		//transform.position = new Vector3(touchWorldPosition.x, touchWorldPosition.y, transform.position.z);
		//float totalFraction = difference.magnitude / t.maxSwipeDistance;
		float totalFraction = t.currentDirection.magnitude / t.maxSwipeDistance;
		float fraction = totalFraction;
		//print(fraction);
		if (fraction > 1f) { fraction = 1f; }
		if(totalFraction > 1.25f) { totalFraction = 1.25f; }
		//Vector2 direction = difference.normalized;
		Vector2 direction = t.currentDirection.normalized;
		Vector2 directionToDot = (direction * fraction * MainScript.blockWidth * 2.5f);
		Vector2 completeDirectionToDot = (direction * totalFraction * MainScript.blockWidth * 2.5f);
		
		dot.position = transform.position + (Vector3)completeDirectionToDot;
		if(drawArrow)
		{
			//MainScript.staticString = "drawing arrows and THE ARROW COUNT IS " + arrows.Count; 
			for(int i = 0; i < arrows.Count;i++)
			{
				//float currentDistance = t.maxSwipeDistance;
				float currentFraction = (float)i / 3f;
				arrows[i].position = transform.position + (Vector3)(directionToDot * currentFraction) ;
			}
		}
	}
	void Start()
    {
		//DrawCircle();
    }
	public void DrawCircle()
	{
		MainScript.blockWidth = (Screen.width / 20f)  * 0.01f;
		//arrows = new List<Transform>();
		int numberOfDots = 12;
		float fraction = (Mathf.PI * 2f) / (float)numberOfDots;
		for (int i = 0; i < numberOfDots; i++)
		{
			Vector3 currentClockhand = new Vector3(Mathf.Sin(fraction * (float)i), Mathf.Cos(fraction * (float)i), 0f).normalized;
			//print(currentClockhand);
			
			Vector3 position = transform.position + (currentClockhand * MainScript.blockWidth * 2f);
			Transform temp = Instantiate(dotPrefab, position, Quaternion.identity).transform;
			float widthScale = MainScript.blockWidth / 0.16f;
			temp.localScale = new Vector3(widthScale,widthScale, 1f);
			temp.parent = transform;
			allSprites.Add(temp.GetComponent<SpriteRenderer>());
			//temp.parent = transform;
			//arrows.Add(temp);
		}
		
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
