﻿/// <summary>
/// Replaces col.rgb with the three colors if they are within the tolerance
/// </summary>
/// <param name="col">The original texture of the image</param>
/// <param name="rTarget">What to replace the reds in the image with</param>
/// <param name="gTarget">What to replace the greens in the image with</param>
/// <param name="bTarget">What to replace the blues in the image with</param>
/// <param name="tolerance">Value from 0-1 determinining strength of replace. Higher values means more likely to replace the color.</param>
/// <returns>The col with all the colors replaced</returns>
fixed4 replaceRGBColors (fixed4 col, fixed4 rTarget, fixed4 gTarget, fixed4 bTarget, float tolerance)
{
	fixed4 c = col;

	//if transparent
	if (c.a==0)
    {
		return fixed4(0, 0, 0, 0);
    }

	//this is the start of the color replace

	//reference to red green and blue colors
	fixed4 r = fixed4(1, 0, 0, 1);
	fixed4 g = fixed4(0, 1, 0, 1);
	fixed4 b = fixed4(0, 0, 1, 1);

	//gets the difference between the colors (smaller difference means color is a closer match and should be replaced)
	float rDiff = length(c - r);
	float gDiff = length(c - g);
	float bDiff = length(c - b);

	//the value of the smallest diff
	float minDiff = min(min(rDiff, gDiff), bDiff);

	//if rDiff is less than tolerance and red is the closest color
	if (rDiff < tolerance && rDiff == minDiff)
	{
		c = rTarget;
	}
	// color swap for 2nd color
	else if (gDiff < tolerance && gDiff == minDiff)
	{
		c = gTarget;
	}

	//color swap for 3rd color
	if (bDiff < tolerance && bDiff == minDiff)
	{
		c = bTarget;
	}

	return c;
}
