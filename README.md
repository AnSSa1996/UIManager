# UIManager


# 사용한 패키지
* UniTask를 사용해서 구현했습니다. 프로젝트에 [UniTask를 임포트 하셔야 합니다.](https://github.com/Cysharp/UniTask/tree/master)
* Dotween을 사용해서 구현했습니다. 프로젝트에 [Dotween을 임포트 하셔야 합니다.](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676?srsltid=AfmBOor0fxxSxt1tKlfsvUCwrb6EZWI2A0Vlm5835ZVEnW8S4h2mxEuT)
* Addressable을 사용해서 구현했습니다. 유니티 패키지에서 어드레서블을 임포트하셔야합니다.
  


# 특징
* 3D 단일 카메라를 사용해서 구현했습니다.
* 배칭을 극대화 하기 위해, 단일 캔버스를 사용해서 구현했습니다. (간단한 프로젝트를 위한, UIManager)
* 자동 바인딩. (enum)
* Prefab이름과 클래스의 이름이 같아야합니다. 클래스의 이름으로 프리팹을 찾습니다. (클래스 프리팹 1:1 매칭)
* 프리팹과 Class 이름을 1:1 대칭시켜 Class 이름으로 프리팹 로드 및 클래스 자동 추가 가능합니다.
* ScreenUI, PopupUI, WorldUI 각종 UI들의 우선 순위 및 열기, 닫기, 찾기 시스템.
* EX) UIFormManager.Instance.OpenUI<TestUI>() -> TestUI 프리팹 오픈.


# 사용 영상

https://github.com/user-attachments/assets/3980da59-576a-487d-b78a-d84316bb4be3




# 사용 방법

- 기본적으로 ResourceManager 셋팅하셔야합니다.

![image](https://github.com/user-attachments/assets/1308be0b-fce7-445d-9ac1-703fe8697a10)


1. @UIManager 프리팹을 첫 씬에 배치합니다. (프로젝트가 처음 시작되는 StartScene)

![image](https://github.com/user-attachments/assets/36db1402-3c08-4477-864b-bc88420190ed)

2. MainCamera UICamera 셋팅하기.

![image](https://github.com/user-attachments/assets/0e3fb2f5-952b-46d3-894d-ee5d15213b82)

3. UI Scripts 및 UI Prefab 작성하기.

![image](https://github.com/user-attachments/assets/8bb84f8e-6743-439a-b777-77c84344d27d)
![image](https://github.com/user-attachments/assets/2f7029ff-ab04-44a9-95d9-e70600c7e8b6)
