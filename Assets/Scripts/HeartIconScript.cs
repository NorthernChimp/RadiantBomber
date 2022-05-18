using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartIconScript : MonoBehaviour
{
	// Start is called before the first frame update
	public HeartIcon thisHeartIcon;
	public Sprite fullHeart;
	public Sprite emptyHeart;
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class HeartIcon
{
	public bool isFull;
	public SpriteRenderer renderer;
	public Transform theTransform;
	public Sprite fullHeart;
	public Sprite emptyHeart;
	public HeartIcon(bool nextState, SpriteRenderer theRenderer, Transform iconTransform)
	{
		theTransform = iconTransform;
		HeartIconScript tempScript = theTransform.GetComponent<HeartIconScript>();
		fullHeart = tempScript.fullHeart;
		emptyHeart = tempScript.emptyHeart;
		renderer = theRenderer;
		//Material temp = (Material)Resources.Load("Textures/FullHealthBlock");
		//renderer.material = temp;
		//renderer.material.SetTexture(temp);
		//fullHeart = Resources.Load("Textures/FullHealthBlock") as Sprite;
		//emptyHeart = Resources.Load("Textures/EmptyHealthBlock") as Sprite;
		ChangeState(nextState);
	}
	public void ChangeState(bool nextState)
	{
		isFull = nextState;
		if (isFull)
		{
			renderer.sprite = fullHeart;
		}
		else
		{
			renderer.sprite = emptyHeart;
		}
	}
}