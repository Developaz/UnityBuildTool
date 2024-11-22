# UnityBuildTool
## 1. Intro
- 다양한 환경변수를 ScriptableObject에 담아서 Unity에서 자동으로 빌드할 수 있는 툴 제작
- 환경 : Unity 2018.4.7f1
## 2. ScriptableObjects
Dev -> Create -> 필요 ScriptableObject 생성
#### 1) BuildInfo.asset
실제 빌드를 하는 정보를 담은 Asset</br></br>
![image](https://github.com/user-attachments/assets/39e05d43-55d1-44fe-aefd-9c2d9eb6c6a3)
- Store : store 정보( 구글, ios, 원스토어 )
- StartServer : 선택된 서버( dev, qa, staging, review, live )
- Lang : 언어( kr, en, jp )
- PublicKey : 스토어 퍼블릭키( 원스토어에서만 사용 )
- AdmobID : 광고 ID
- AdmobKeyMain : 광고 key의 앞부분( ex : ca-app-pub-9429808070250988/1150010542 이라면 ca-app-pub-9429808070250988 부분 )
- AdRewardKey : 광고 key list
#### 2) StoreInfo.asset
각 스토어 별로 BuildInfo에 들어갈 정보를 담은 Asset</br></br>
![image](https://github.com/user-attachments/assets/8a236270-dcdd-4543-b695-001e8d725099)
- AppName : 앱 이름
- Package : 패키지명
- Version : 게임 버전
- BundleVersion : 게임 번들 버전
- PublicKey : 스토어 퍼블릭키( 원스토어에서만 사용 )
- AdmobID : 광고 ID
- AdmobKeyMain : 광고 key의 앞부분( ex : ca-app-pub-9429808070250988/1150010542 이라면 ca-app-pub-9429808070250988 부분 )
- AdRewardKey : 광고 key list
- Icons : 앱 아이콘
#### 3) BuildPath.asset
빌드 파일이 생성될 위치 정보 Asset</br></br>
![image](https://github.com/user-attachments/assets/2e713e6c-54a1-4e7f-a81c-c450bc81965f)
- GoogleStoreAPKPath : 구글 스토어용 APK 파일 생성 위치
- OneStoreAPKPath : 원스토어용 APK 파일 생성 위치
- XcodeProjectPath : 앱스토어용 Xcode Project 생성 위치
## 3. BuildTool
Dev -> BuildTool</br>
![image](https://github.com/user-attachments/assets/5ccd30d7-f26c-4223-9789-7d55a99a2743)
- Select Store : 빌드 타겟 Store
- Select Server : 빌드 타겟 Server
- Select Language : 언어 설정
- Version : 게임 버전
- BundleVersion : 게임 번들 버전
- Split Binary : Split Binary Option 여부
- Load Button : BuildInfo.asset에 있는 정보 Load
- Icons : PlayerSetting에 들어갈 Icon Image들
- Build : 빌드 실행
