using UnityEngine;
using UnityEngine.UI;
/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
public class RadialLayout : LayoutGroup
{
	//// Fields ////
	[ Range( 0.0f, 500.0f ) ]
	public	float			m_fDistance;
	[ Range( 0.0f, 360.0f ) ]
	public	float			m_startAngle;

	private const float		m_maxAngle = 360.0f;
	private float			m_minAngle;
	private int				m_numActiveChildren;

	//// Unity Callbacks ////
	protected override void OnEnable()
	{
		base.OnEnable();

		CalculateRadial();
	}
	public override void SetLayoutHorizontal()
	{
	}
	public override void SetLayoutVertical()
	{
	}
	public override void CalculateLayoutInputVertical()
	{
		CalculateRadial();
	}
	public override void CalculateLayoutInputHorizontal()
	{
		CalculateRadial();
	}
#if UNITY_EDITOR
	//protected override void OnValidate()
	//{
	//	base.OnValidate();

	//	CalculateRadial();
	//}
#endif

	//// Other Methods ////
	private void DetermineNumberOfActiveChildren()
	{
		m_numActiveChildren = 0;
		for( int childIdx = 0; childIdx < transform.childCount; childIdx++ )
		{
			var child = transform.GetChild( childIdx );
			if( child && child.gameObject.activeSelf )
			{
				m_numActiveChildren++;
			}
		}
	}
	void CalculateRadial()
	{
		DetermineNumberOfActiveChildren();
		m_minAngle = 360.0f / m_numActiveChildren;

		m_Tracker.Clear();
		if( m_numActiveChildren == 0 )
		{
			return;
		}

		float fOffsetAngle = ( ( m_maxAngle - m_minAngle ) ) / ( m_numActiveChildren - 1 );

		float fAngle = m_startAngle;
		for( int childIdx = 0; childIdx < transform.childCount; childIdx++ )
		{
			RectTransform child = ( RectTransform )transform.GetChild( childIdx );
			if( child != null && child.gameObject.activeSelf )
			{
				// Adding the elements to the tracker stops the user from modifiying their positions via the editor.
				m_Tracker.Add( this, child,
				DrivenTransformProperties.Anchors |
				DrivenTransformProperties.AnchoredPosition |
				DrivenTransformProperties.Pivot );
				Vector3 vPos = new Vector3( Mathf.Cos( fAngle * Mathf.Deg2Rad ), Mathf.Sin( fAngle * Mathf.Deg2Rad ), 0 );
				child.localPosition = vPos * m_fDistance;
				// Force objects to be center aligned, 
				// this can be changed however I'd suggest you keep all of the objects with the same anchor points.
				child.anchorMin = child.anchorMax = child.pivot = new Vector2( 0.5f, 0.5f );
				fAngle += fOffsetAngle;
			}
		}
	}
}