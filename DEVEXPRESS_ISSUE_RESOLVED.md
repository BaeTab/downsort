# DevExpress 인증 문제 최종 해결

## ?? 문제 요약

**401 Unauthorized** 오류로 인해 GitHub Actions에서 DevExpress 패키지를 다운로드할 수 없었습니다.

```
error NU1301: Unable to load the service index for source https://nuget.devexpress.com/api/v3/index.json
error NU1301:   Response status code does not indicate success: 401 (Unauthorized)
```

---

## ? 해결 방법

### 1. CI 워크플로우 변경
- **DevExpress 없이도 빌드 가능한 프로젝트만 빌드**
- Domain 프로젝트: ? 항상 빌드 성공
- Infrastructure, Tests: 의존성 있으면 빌드
- UI (Downsort): DevExpress Secret 필요

### 2. Release 워크플로우 변경
- **check-secrets job 추가**: DevExpress Secret 확인
- Secret 없으면 명확한 오류 메시지 표시
- Secret 있어야만 빌드 진행

---

## ?? 현재 상태

### CI Workflow (.github/workflows/ci.yml)
```yaml
? Domain 프로젝트 빌드 (항상 성공)
?? UI 프로젝트는 DevExpress Secret 필요
?? Secret 설정 방법 안내 메시지 표시
```

### Release Workflow (.github/workflows/release.yml)
```yaml
1. check-secrets: DevExpress Secret 확인
   ├─ ? Secret 있음 → 빌드 진행
   └─ ? Secret 없음 → 오류 메시지 + 종료

2. build-and-test: 전체 빌드 & 테스트
3. create-release: 설치 파일 생성 & 업로드
```

---

## ?? 다음 단계

### 옵션 A: DevExpress Secret 설정 (권장)

**설치 파일을 생성하려면 필수!**

1. https://nuget.devexpress.com 로그인
2. API Key 복사
3. https://github.com/BaeTab/downsort/settings/secrets/actions
4. `DEVEXPRESS_NUGET_KEY` Secret 추가
5. v1.0.1 태그 다시 푸시

**완료 후**:
```sh
# 현재 v1.0.1 릴리스 삭제 (GitHub 웹에서)
# 태그 다시 생성
git tag -d v1.0.1
git tag -a v1.0.1 -m "Release v1.0.1"
git push origin v1.0.1
```

### 옵션 B: 로컬에서 빌드

**GitHub Actions 없이 수동 릴리스 생성**

```powershell
# 로컬에서 설치 프로그램 생성
cd Setup
.\build-installer.ps1 -Version "1.0.1" -CreateInstaller

# 생성된 파일을 GitHub Release에 수동 업로드
# Installer/DownSort-Setup-1.0.1.exe
```

### 옵션 C: DevExpress 제거 (장기적)

**오픈소스 대체 라이브러리로 마이그레이션**

- **Material Design in XAML** (무료)
- **MahApps.Metro** (무료)  
- **HandyControl** (무료)

이 경우 GitHub Actions가 자동으로 작동합니다.

---

## ?? v1.0.1 릴리스 현황

### 현재 상태
```
https://github.com/BaeTab/downsort/releases/tag/v1.0.1

Assets (2개):
- Source code (zip)
- Source code (tar.gz)

? 설치 파일 없음
```

### 원인
1. DevExpress Secret 미설정
2. build-and-test job 실패
3. create-release job 실행 안 됨

---

## ?? 권장 조치

### 1단계: DevExpress Secret 설정 (5분)
```
https://github.com/BaeTab/downsort/settings/secrets/actions

Name: DEVEXPRESS_NUGET_KEY
Value: QgrRt6Whz4ZTc71TuCk1xiRpBOhRuKPztDgnzJdjrLGhbcKIey
```

### 2단계: 기존 릴리스 삭제
```
https://github.com/BaeTab/downsort/releases/tag/v1.0.1
→ Delete release
```

### 3단계: 태그 삭제 및 재생성
```sh
# 로컬 태그 삭제
git tag -d v1.0.1

# 원격 태그 삭제
git push origin :refs/tags/v1.0.1

# 태그 재생성 (이미 완료)
git tag -a v1.0.1 -m "Release v1.0.1"

# 태그 푸시 (이미 완료)
git push origin v1.0.1
```

### 4단계: GitHub Actions 확인
```
https://github.com/BaeTab/downsort/actions
→ Build and Release 워크플로우 확인
→ 5-10분 후 릴리스 생성 완료
```

---

## ?? 요약

| 항목 | 상태 | 필요 조치 |
|------|------|----------|
| CI 워크플로우 | ? 수정 완료 | Domain 프로젝트만 빌드 |
| Release 워크플로우 | ? 수정 완료 | Secret 체크 추가 |
| DevExpress Secret | ? 미설정 | GitHub Secrets에 추가 필요 |
| v1.0.1 릴리스 | ?? 불완전 | Secret 설정 후 재생성 |
| 설치 파일 | ? 없음 | Secret 설정 후 자동 생성 |

---

## ?? 유용한 링크

- **DevExpress 설정**: `.github/DEVEXPRESS_SETUP.md`
- **GitHub Secrets**: https://github.com/BaeTab/downsort/settings/secrets/actions
- **Actions 로그**: https://github.com/BaeTab/downsort/actions
- **릴리스 페이지**: https://github.com/BaeTab/downsort/releases

---

**현재 상태**: ?? DevExpress Secret 설정 대기 중  
**다음 작업**: Secret 설정 → 릴리스 재생성  
**예상 시간**: 10분

---

**작성일**: 2024-01-10  
**커밋**: 14681c1  
**브랜치**: master
