# Stats

base value와 modifier를 기반으로 스탯을 계산하고, 현재/최대값 형태의 리소스(체력, 마나 등)를 관리합니다. Stat에 modifier를 추가/제거하면 Resource의 최대값도 자동으로 반영됩니다.

[ Usage ](#usage) <br>
[ API ](#api)

<br>
<br>
<br>


## 🔧Usage

<br>

#### Stat 생성
```cs

var attackPower = new Stat("attack_power", baseValue: 10f);

```

key는 string으로 식별합니다.

<br>
<br>

#### Modifier 추가/제거
```cs

var weaponMod = new StatModifier(5f, StatModType.Flat, source: weaponInstance);
attackPower.AddModifier(weaponMod);

attackPower.RemoveModifier(weaponMod.Id);

// 장비 해제, 버프 만료 등 특정 출처의 modifier를 일괄 제거
attackPower.RemoveAllFromSource(weaponInstance);

```

<br>
<br>

#### 값 조회 및 변경 감지
```cs

attackPower.Value; // 계산된 최종값 (내부적으로 dirty flag 캐싱)

attackPower.OnValueChanged += newValue =>
{
    // UI 갱신 등
};

```

<br>
<br>

#### Resource 생성 및 사용
```cs

var maxHp = new Stat("max_hp", baseValue: 100f);
var hp = new Resource("hp", maxHp);

hp.Modify(-30f);      // 데미지
hp.SetCurrent(50f);   // 직접 설정

hp.OnChanged += (current, max) =>
{
    // 체력바 갱신
};

hp.OnDepleted += () =>
{
    // 사망 처리
};

```

`maxHp`에 modifier가 추가/제거되면 `hp.Max`가 자동으로 갱신되고, `Current`는 새 `Max`를 넘지 않도록 clamp됩니다.

<br>
<br>


## 📖API

#### Stat
**`BaseValue`** : 기본값입니다.<br>
**`Value`** : modifier가 모두 반영된 최종값입니다. 변경이 있을 때만 재계산됩니다.<br>
**`Modifiers`** : 현재 적용된 modifier 목록입니다.<br>
**`SetBaseValue(value)`** : 기본값을 변경합니다.<br>
**`AddModifier(modifier)`** : modifier를 추가합니다.<br>
**`RemoveModifier(id)`** : 해당 id의 modifier를 제거합니다.<br>
**`RemoveAllFromSource(source)`** : 해당 source가 부여한 모든 modifier를 일괄 제거합니다.<br>
**`OnValueChanged`** : `Value`가 변경될 때 발생하는 이벤트입니다.<br>

<br>

#### StatModifier
**`Id`** : modifier 고유 식별자입니다. 개별 제거 시 사용합니다.<br>
**`Value`** : 적용할 수치입니다.<br>
**`Type`** : 적용 방식입니다 (`Flat` / `PercentAdd` / `PercentMult`).<br>
**`Order`** : 적용 순서입니다. 기본값은 `Type`의 내부 값을 따릅니다.<br>
**`Source`** : modifier의 출처입니다. 장비, 버프 등 일괄 제거 기준으로 사용됩니다.<br>

계산 순서 : `Flat`을 모두 더한 뒤 → 연속된 `PercentAdd`는 합산하여 한 번에 곱하고 → `PercentMult`는 각각 곱합니다.

<br>

#### Resource
**`Key`** : 리소스 식별자입니다.<br>
**`MaxStat`** : 최대값으로 참조하는 Stat입니다.<br>
**`Current`** : 현재값입니다.<br>
**`Max`** : `MaxStat.Value`를 그대로 따라갑니다.<br>
**`Modify(delta)`** : 현재값을 delta만큼 증감합니다 (0~Max로 clamp).<br>
**`SetCurrent(value)`** : 현재값을 직접 설정합니다.<br>
**`OnChanged`** : `Current` 또는 `Max`가 변경될 때 발생하는 이벤트입니다 (current, max).<br>
**`OnDepleted`** : `Current`가 0에 도달했을 때 발생하는 이벤트입니다.<br>

<br>
<br>
<br>
