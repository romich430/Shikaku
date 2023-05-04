using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class TowerBlockManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private BoxCollider collider;
    private BlockTowerController parent;
    public Vector3 basePoint;

    public void SetMaterial(Material mat)
    {
        mesh.material = mat;
    }
    public void SetColliderActive(bool value)
    {
        collider.enabled = value;
    }

    public bool MoveToSpot(Vector3 spot)
    {
        Vector3 spotXY= spot;
        if (spot.z > transform.position.z)
        {
            spotXY = new Vector3(spot.x, spot.y, transform.position.z);
            var sequence = DOTween.Sequence()
                .Append(transform.DOMove(spotXY, 0.2f)).SetEase(Ease.InOutCubic)
                .AppendInterval(0.3f)
                .Append(transform.DOMove(spot, 0.4f)).SetEase(Ease.Linear);
        }
        else
        {
            spotXY = new Vector3(transform.position.x, transform.position.y, spot.z);
            var sequence = DOTween.Sequence()
                .Append(transform.DOMove(spotXY, 0.4f)).SetEase(Ease.Linear)
                .AppendInterval(0.3f)
                .Append(transform.DOMove(spot, 0.2f)).SetEase(Ease.InOutCubic);
        }
        return true;
    }

    public void SetupBlock(Material mat, Vector3 spot, BlockTowerController parent_, bool disableCollider = true)
    {
        basePoint = spot;
        SetMaterial(mat);
        SetColliderActive(!disableCollider);
        parent = parent_;
    }
    
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.collider == collider)
            {
                ProccessInput();
            }
        }
    }

    private void ProccessInput()
    {
        parent.Fold();
    }
}
