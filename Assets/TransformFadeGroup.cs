using UniRx;
using UnityEngine;

public class TransformFadeGroup : MonoBehaviour
{

    public ReactiveProperty<float> _spriteAlpha { get; set; } = new ReactiveProperty<float>(0f);
    private SpriteRenderer[] _sprites;

    private void Awake()
    {

        _sprites = this.GetComponentsInChildren<SpriteRenderer>();

        _spriteAlpha
           .DistinctUntilChanged()
           .Subscribe(spriteAlpha =>
           {
               foreach (var sprite in _sprites)
               {
                   Color color = sprite.color;
                   color.a = _spriteAlpha.Value;
                   sprite.color = color;
               }

           })
           .AddTo(this);
    }

   
   
}
