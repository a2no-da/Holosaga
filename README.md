<div align="center">

# HOLOSAGA: Invasion of the HoloX

### 홀로라이브 IP 기반 2D 라인 디펜스 게임

기획자 1명과 단둘이 15개월간 개발하여 Steam 정식 출시까지 완료
평가: **매우 긍정적**

![Unity](https://img.shields.io/badge/Unity-2022.3.8f1-000000?logo=unity)
![C#](https://img.shields.io/badge/C%23-Unity_Client-239120?logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6)
![Release](https://img.shields.io/badge/Status-Released_on_Steam-success)
[![Steam](https://img.shields.io/badge/Steam-Store_Page-1b2838?logo=steam)](https://store.steampowered.com/app/2899450/HOLOSAGA_Invasion_of_the_HoloX/)

## 목차

- [소개](#소개)
- [담당 기능](#담당-기능)
- [핵심 기술 및 문제 해결](#핵심-기술)
- [한계 및 아쉬운 점](#한계)
- [링크](#링크)

---

<img width="1144" height="637" alt="title" src="https://github.com/user-attachments/assets/32642660-93cb-46ee-be0a-3bfc97342865" />
<img width="805" height="450" alt="screenshot_boss2" src="https://github.com/user-attachments/assets/8099b9fe-9dcc-4ec7-be43-353d0027f2db" />
<img width="805" height="456" alt="screenshot_boss5" src="https://github.com/user-attachments/assets/167ca723-b5b0-4369-8368-24a7fc9c41a6" />
<img width="804" height="452" alt="screenshot_collection" src="https://github.com/user-attachments/assets/c6359cf1-9708-4b82-9595-90bc61ecf523" />
<img width="804" height="452" alt="gameplay" src="https://github.com/user-attachments/assets/d49805fe-0347-4fdb-bda4-c46a394254ce" />


<a id="소개"></a>
## 소개

4개의 라인으로 몰려오는 적과 패턴을 가진 보스를 실시간 유닛 조작으로 막아내는 라인 디펜스 게임입니다. 홀로라이브 버추얼 아이돌 IP를 기반으로 하며, 20개 이상의 캐릭터가 각자 다른 스킬·아티팩트·버프를 가지는 수집형 시스템을 포함합니다.

| 항목 | 내용 |
|---|---|
| 플랫폼 | PC (Windows) |
| 엔진 / 언어 | Unity 2022.3.8f1 / C# |
| 개발 인원 | 2명 (기획 1, 프로그래밍 1) |
| 개발 기간 | 2023.11 ~ 2025.01 (15개월) |
| 담당 | 클라이언트 프로그래밍 전반 |

---

<a id="담당-기능"></a>
## 담당 기능

클라이언트 프로그래밍 전반을 담당했습니다. 보스 패턴/쿨타임 시스템, 캐릭터별 아티팩트·버프 적용 로직, CSV 기반 데이터 파이프라인, 세이브 데이터 직렬화/암호화, Steamworks(업적·Steam Cloud) 연동까지 — 게임 로직이 실제로 동작하는 부분을 혼자 설계하고 구현했습니다.

<a id="핵심-기술"></a>
## 핵심 기술 및 문제 해결

1. 보스 다중 패턴 쿨타임 관리

보스는 처음부터 패턴 5개(텔레포트 포함)로 설계했고, 패턴 1·2는 모든 보스 공통, 3~5는 스테이지 보스마다 다르게 구현했습니다. 패턴마다 쿨타임뿐 아니라 "다른 패턴 진행 중에도 쿨타임이 줄어드는지"(독립 여부), "사거리 밖에서도 발동하는지"(사거리 무시 여부) 조건까지 달라서, 이 조건들을 CSV(MonsterCooltime)의 IsIndependent/IgnoreRange 값으로 읽어와 보스 필드에 매핑하고, 매 프레임 조건에 따라 각 타이머를 개별 감소시키다가 발동 조건을 만족하면 트리거하는 방식으로 구현했습니다.

다만 Pattern1Timer ~ Pattern5Timer처럼 필드가 패턴 수만큼 그대로 늘어나는 구조라, 신규 보스 추가 시 거의 같은 코드를 반복 작성해야 했습니다. 지금이라면 Dictionary<patternId, PatternState>로 일반화해 데이터만 추가하면 되게 만들었을 것 같습니다.

---

2. 애니메이션 이벤트 기반 텔레포트 동기화

보스 텔레포트 연출에서 애니메이션 재생 타이밍과 실제 위치 이동 타이밍이 어긋나면, 이동이 순간적으로 튀어 보이거나 애니메이션 도중 위치가 먼저 바뀌어버리는 문제가 있었습니다. Spine 애니메이션의 특정 프레임에 커스텀 이벤트(teleport)를 심고, AnimationState.Event 콜백에서만 실제 Transform 이동을 실행하도록 로직을 분리해 해결했습니다. 이동 위치는 현재 위치와 같은 라인(y좌표)을 제외한 지점 중 무작위로 골라, 같은 줄로 재텔레포트되는 것도 방지했습니다.

---

3. 아티팩트 / 능력카드 확장 구조

캐릭터마다 고유한 아티팩트·능력카드 효과가 있는데, 이걸 유닛 클래스 안에 조건문으로 직접 넣으면 아티팩트가 하나 추가될 때마다 핵심 로직(Unit, Tower) 코드를 계속 건드려야 합니다. 그래서 각 아티팩트가 자신의 효과를 담당하는 기능 객체를 참조하도록 구성하고, 유닛에서는 해당 기능을 호출하는 역할만 담당하도록 책임을 분리했습니다. 신규 아티팩트를 추가해도 Unit/Tower 코드는 그대로 두고 아티팩트 쪽 함수만 새로 작성하면 됩니다.

---

4. 데이터 외부화 파이프라인

기존에는 캐릭터 스탯을 하나 바꾸려 해도 코드 수정과 재빌드가 필요했습니다. 이를 CSV → Dictionary 기반 동적 로드 + 이벤트 기반 UI 갱신 구조로 바꿔서, 변경 후에는 CSV 데이터 수정만으로 빌드 없이 게임 내 수치에 바로 반영되도록 개선했습니다.

---

5. 세이브 데이터 무결성

업적/컬렉션 데이터 특성상 로컬 저장 데이터의 무결성이 중요했습니다. 평문 JSON으로 저장하면 로컬 파일을 직접 열어 수정할 수 있기 때문에, JSON 직렬화 → AES 암호화 → 로컬 저장 → Steam Cloud 동기화 순서로 저장 구조를 잡아 변조 난이도를 높이고 기기 간 데이터 연속성을 확보했습니다.

---

6. 로케일 버그 트러블슈팅

특정 지역(콤마를 소수점으로 쓰는 로케일, 예: 러시아)에서 세이브 데이터 로드 시 FormatException이 발생해 기존 데이터가 유실되는 문제가 있었습니다. 원인은 데이터 로딩 과정(Stat())에서 float.Parse를 시스템 로케일 기준으로 호출하고 있었기 때문입니다.

직접적인 수정은 모든 수치 데이터 파싱 지점에 CultureInfo.InvariantCulture를 명시적으로 지정한 것입니다.

csharp
centerX = float.Parse(data["CenterPos"].Split(',')[0], CultureInfo.InvariantCulture);

이 문제를 계기로, 텍스트와 수치 데이터가 뒤섞여 있던 부분도 점검하면서 다국어 텍스트는 Unity Localization + ScriptableObject 기반 Key 참조 구조로 분리했습니다. 

---

<a id="한계"></a>
## 한계 및 아쉬운 점

캐릭터별 버프 적용 로직은 switch (buffName) 형태로 캐릭터 이름별 분기가 하드코딩되어 있어, 캐릭터가 50개 이상으로 늘어나며 반복적인 케이스 추가가 계속 필요했습니다. 아티팩트 시스템에 적용한 구조를 버프 시스템에도 동일하게 적용했다면 확장성 문제를 줄일 수 있었을 것 같습니다.

순수 C# 클래스로 처리할 수 있는 로직도 ScriptableObject나 MonoBehaviour의 라이프사이클에 종속시킨 부분이 많았고, Unity가 제공하는 프레임워크에 의존도가 높았습니다. 테스트 용이성이나 엔진 독립성을 고려했다면 일부 로직은 순수 C# 레벨로 분리했을 것 같습니다.

---

<a id="링크"></a>
## 링크

Steam: https://store.steampowered.com/app/2899450/HOLOSAGA_Invasion_of_the_HoloX/
인게임 영상: (링크 추가)

## 핵심 코드 바로가기

| 파일 | 설명 |
|---|---|
| [`BOSS.cs`](Assets/Script/BOSS.cs) | 보스 다중 패턴 쿨타임 관리, 텔레포트 동기화 |
| [`Tower.cs`](Assets/Script/Tower.cs) | 캐릭터 버프/아티팩트 적용 로직 |
| [`Unit.cs`](Assets/Script/Unit.cs) | 캐릭터/유닛 공통 로직, 데이터 파싱 |
| [`Enemy.cs`](Assets/Script/Enemy.cs) | 적 유닛 이동/행동 로직 |

---

## 폴더 구조

<div align="left">
  
```
📁 Assets/
├── 📁 Script/             # 게임 로직 전체
│   ├── 📄 BOSS.cs           # 보스 패턴 
│   ├── 📄 Enemy.cs          # 적 유닛 로직
│   ├── 📄 Tower.cs          # 타워(캐릭터) 로직, 버프/아티팩트 적용
│   ├── 📄 Unit.cs           # 캐릭터/유닛 공통 로직
│   └── 📁 데이터/           # CSV 파싱 및 데이터 관리
├── 📁 Resources/            # 런타임 로드 데이터 (CSV 등)
├── 📁 Scenes/               # 씬 파일
├── 📁 Language/             # 로컬라이제이션 데이터
├── 📁 AddressableAssetsData/
└── 📁 Spine/                # 2D 스켈레탈 애니메이션 리소스
```

<div align="center">
게임 스크립트는 Assets/Script 하위에 있습니다.

**HOLOSAGA: Invasion of the HoloX** · 2인 개발 · 15개월 · Steam 정식 출시

</div>
