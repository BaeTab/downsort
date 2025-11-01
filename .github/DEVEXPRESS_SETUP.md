# DevExpress NuGet Feed 설정 가이드

## 문제

GitHub Actions에서 DevExpress 패키지를 찾을 수 없는 오류 발생:

```
error NU1101: Unable to find package DevExpress.Wpf
error NU1102: Unable to find package DevExpress.Mvvm with version (>= 25.1.0)
error NU1101: Unable to find package DevExpress.Wpf.ThemesLW
```

**원인**: DevExpress는 상용 라이브러리로 공개 NuGet 저장소에 없습니다.

---

## 해결 방법

### 1. nuget.config 생성 (완료)
프로젝트 루트에 `nuget.config` 파일이 생성되었습니다.

### 2. GitHub Secrets 설정 (필수)

DevExpress NuGet feed 인증을 위해 GitHub Secrets를 추가해야 합니다:

#### 2.1. DevExpress 계정 정보 확인
1. https://nuget.devexpress.com 로그인
2. "Obtain Feed URL" 클릭
3. Username과 API Key 확인

#### 2.2. GitHub Secrets 추가
1. GitHub 리포지토리 페이지로 이동
```
https://github.com/BaeTab/downsort
```

2. **Settings** → **Secrets and variables** → **Actions** 클릭

3. **New repository secret** 클릭하여 다음 2개의 시크릿 추가:

| Secret Name | Value | 설명 |
|-------------|-------|------|
| `DEVEXPRESS_NUGET_USERNAME` | DevExpress 이메일 | NuGet feed 인증 사용자명 |
| `DEVEXPRESS_NUGET_KEY` | API Key | DevExpress NuGet API Key |

---

## 설정 방법 (단계별)

### Step 1: DevExpress API Key 가져오기

1. https://nuget.devexpress.com 로그인
2. **Obtain Feed URL** 버튼 클릭
3. 다음 정보 복사:
   - **Username**: 이메일 주소
   - **API Key**: 긴 문자열 (예: `xxx-xxx-xxx-xxx`)

### Step 2: GitHub Secrets 추가

1. GitHub 리포지토리로 이동:
```
https://github.com/BaeTab/downsort/settings/secrets/actions
```

2. **New repository secret** 클릭

3. 첫 번째 Secret 추가:
```
Name: DEVEXPRESS_NUGET_USERNAME
Value: your-email@example.com
```
**Add secret** 클릭

4. 두 번째 Secret 추가:
```
Name: DEVEXPRESS_NUGET_KEY
Value: your-api-key-here
```
**Add secret** 클릭

---

## 확인 방법

### 1. Secrets 설정 확인

GitHub 리포지토리에서:
```
Settings > Secrets and variables > Actions
```

다음 2개의 Secret이 표시되어야 합니다:
- ? `DEVEXPRESS_NUGET_USERNAME`
- ? `DEVEXPRESS_NUGET_KEY`

### 2. 워크플로우 테스트

```sh
# 새로운 커밋 푸시
git add .
git commit -m "chore: add DevExpress NuGet feed configuration"
git push origin master
```

GitHub Actions가 자동으로 실행되며, DevExpress 패키지를 정상적으로 복원합니다.

---

## 대안 방법 (DevExpress 없이 빌드)

DevExpress 라이선스가 없는 경우, 다음 방법을 사용할 수 있습니다:

### 방법 1: CI/CD 비활성화 (임시)
```yaml
# .github/workflows/ci.yml
on:
  push:
    branches: [ master, main, develop ]
    paths-ignore:
      - '**'  # 모든 파일 무시 (CI 비활성화)
```

### 방법 2: 로컬 빌드만 사용
GitHub Actions를 사용하지 않고, 로컬에서만 빌드 및 릴리스 생성:

```powershell
# 로컬 빌드
dotnet build DownSort.sln --configuration Release

# 설치 프로그램 생성
cd Setup
.\build-installer.ps1 -CreateInstaller

# 수동으로 GitHub Release 생성
```

### 방법 3: DevExpress → 오픈소스 대체
DevExpress WPF 컨트롤을 다음으로 대체:
- **Material Design in XAML** (무료)
- **MahApps.Metro** (무료)
- **HandyControl** (무료)

---

## 문제 해결

### Q1: "Unable to add source" 오류
```
A source with the name 'DevExpress' already exists.
```

**해결**:
```sh
dotnet nuget list source
dotnet nuget remove source DevExpress
```

### Q2: "Unauthorized" 오류
```
401 (Unauthorized)
```

**해결**:
- DevExpress API Key가 올바른지 확인
- GitHub Secrets 값이 정확한지 확인
- DevExpress 계정이 활성화되어 있는지 확인

### Q3: Secrets가 작동하지 않음

**확인 사항**:
1. Secret 이름이 정확한지 확인 (대소문자 구분)
2. Workflow permissions 확인:
```
Settings > Actions > General > Workflow permissions
"Read and write permissions" 선택
```

---

## 로컬 개발 환경 설정

### Windows (로컬)

1. **NuGet.config 수동 설정**:
```xml
<!-- %AppData%\NuGet\NuGet.Config -->
<configuration>
  <packageSources>
    <add key="DevExpress" value="https://nuget.devexpress.com/api/v3/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <DevExpress>
      <add key="Username" value="your-email@example.com" />
      <add key="ClearTextPassword" value="your-api-key" />
    </DevExpress>
  </packageSourceCredentials>
</configuration>
```

2. **Visual Studio에서 설정**:
   - Tools → NuGet Package Manager → Package Manager Settings
   - Package Sources
   - Add new source:
     - Name: `DevExpress`
     - Source: `https://nuget.devexpress.com/api/v3/index.json`
     - Username: 이메일
     - Password: API Key

---

## 워크플로우 동작 방식

### CI Workflow
```yaml
1. Setup DevExpress NuGet source
   ├─ dotnet nuget add source
   ├─ Username: ${{ secrets.DEVEXPRESS_NUGET_USERNAME }}
   └─ Password: ${{ secrets.DEVEXPRESS_NUGET_KEY }}

2. Restore dependencies
   └─ DevExpress 패키지 다운로드

3. Build & Test
```

### Release Workflow
```yaml
1. Setup DevExpress NuGet source (CI와 동일)
2. Restore & Build
3. Publish application
4. Create installer
5. Create GitHub Release
```

---

## 참고 자료

- [DevExpress NuGet Feed](https://nuget.devexpress.com/)
- [GitHub Secrets 문서](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [NuGet.config 문서](https://docs.microsoft.com/en-us/nuget/reference/nuget-config-file)

---

## 체크리스트

- [ ] DevExpress 계정 로그인
- [ ] API Key 확인
- [ ] GitHub Secrets 추가 (DEVEXPRESS_NUGET_USERNAME)
- [ ] GitHub Secrets 추가 (DEVEXPRESS_NUGET_KEY)
- [ ] 변경사항 커밋 및 푸시
- [ ] GitHub Actions 실행 확인
- [ ] 빌드 성공 확인

---

**작성일**: 2024-01-10  
**업데이트**: 2024-01-10  
**작성자**: GitHub Copilot
