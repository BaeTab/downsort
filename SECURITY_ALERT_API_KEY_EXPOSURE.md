# ?? 보안 경고: API Key 노출 및 조치 사항

## ?? 심각도: 높음

### 문제
DevExpress API Key가 `nuget.config` 파일에 하드코딩되어 Git에 커밋되었습니다.

```xml
<!-- 노출된 API Key (이미 제거됨) -->
<add key="DevExpress" value="https://nuget.devexpress.com/QgrRt6Whz4ZTc71TuCk1xiRpBOhRuKPztDgnzJdjrLGhbcKIey/api/v3/index.json" />
```

**커밋**: `e28700a` (2024-01-10)에서 제거됨

---

## ? 즉시 조치 사항

### 1. DevExpress API Key 재생성 (필수!)

**노출된 API Key는 더 이상 안전하지 않습니다.**

#### 재생성 방법:
1. https://nuget.devexpress.com 로그인
2. **"Regenerate Feed"** 또는 **"Regenerate API Key"** 클릭
3. 새로운 API Key 생성 확인
4. 새로운 API Key 복사

---

### 2. 로컬 개발 환경 업데이트

**새로운 API Key로 로컬 NuGet source 업데이트:**

```powershell
# 기존 source 제거
dotnet nuget remove source DevExpress

# 새로운 API Key로 source 추가
dotnet nuget add source "https://nuget.devexpress.com/{NEW_API_KEY}/api/v3/index.json" --name DevExpress

# 패키지 복원
dotnet restore DownSort.sln
```

---

### 3. GitHub Secrets 업데이트

**GitHub Actions를 위한 Secret 업데이트:**

1. https://github.com/BaeTab/downsort/settings/secrets/actions
2. `DEVEXPRESS_NUGET_KEY` Secret 편집
3. 새로운 API Key로 업데이트
4. **Save** 클릭

---

## ?? 영향 범위

### Git 히스토리
```
?? 노출된 API Key가 Git 히스토리에 남아있습니다.
   완전히 제거하려면 Git 히스토리를 재작성해야 합니다.
```

### 커밋 로그
```sh
# API Key가 포함된 커밋 확인
git log --all --full-history -- nuget.config

# 결과:
# commit 14681c1... (API Key 추가)
# commit e28700a... (API Key 제거) ← 현재
```

---

## ??? Git 히스토리 재작성 (고급)

**완전한 보안을 위해 Git 히스토리에서 API Key 제거:**

### 옵션 1: BFG Repo-Cleaner (권장)

```sh
# BFG 설치
# https://rtyley.github.io/bfg-repo-cleaner/

# API Key 제거
bfg --replace-text passwords.txt

# Force push
git push --force origin master
```

### 옵션 2: git filter-repo

```sh
# git-filter-repo 설치
pip install git-filter-repo

# API Key 제거
git filter-repo --replace-text passwords.txt

# Force push
git push --force origin master
```

### 옵션 3: GitHub Support

**GitHub에 연락하여 캐시된 내용 삭제 요청:**
- https://support.github.com
- "Remove sensitive data" 요청

---

## ?? 향후 예방 조치

### 1. .gitignore 업데이트

```gitignore
# Local NuGet configuration with API keys
nuget.config.local
**/nuget.config.user
```

### 2. Pre-commit Hook 설정

```sh
# .git/hooks/pre-commit
#!/bin/sh
if git diff --cached | grep -i "api.*key"; then
    echo "?? Possible API key detected in commit!"
    exit 1
fi
```

### 3. GitHub Secret Scanning

GitHub가 자동으로 API Key를 감지하고 경고합니다.

---

## ?? 보안 체크리스트

### 즉시 (5분 이내)
- [ ] DevExpress API Key 재생성
- [ ] 로컬 NuGet source 업데이트
- [ ] GitHub Secrets 업데이트
- [ ] 빌드 및 테스트 확인

### 단기 (24시간 이내)
- [ ] Git 히스토리 재작성 고려
- [ ] GitHub Support에 캐시 삭제 요청
- [ ] 팀원들에게 알림

### 장기
- [ ] Pre-commit hook 설정
- [ ] 보안 정책 문서화
- [ ] 정기 보안 감사

---

## ?? 관련 리소스

### DevExpress
- **API Key 관리**: https://nuget.devexpress.com
- **지원 센터**: https://www.devexpress.com/support

### GitHub
- **Secret Scanning**: https://docs.github.com/en/code-security/secret-scanning
- **Removing Sensitive Data**: https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/removing-sensitive-data-from-a-repository

### 도구
- **BFG Repo-Cleaner**: https://rtyley.github.io/bfg-repo-cleaner/
- **git-filter-repo**: https://github.com/newren/git-filter-repo

---

## ?? 타임라인

| 시간 | 이벤트 | 조치 |
|------|--------|------|
| 2024-01-10 14:30 | API Key 노출 | nuget.config 커밋 |
| 2024-01-10 14:45 | 발견 | Copilot이 감지 |
| 2024-01-10 14:46 | 제거 | commit e28700a |
| 2024-01-10 14:47 | 푸시 | GitHub에 업로드 |
| **즉시** | **재생성** | **DevExpress API Key** |

---

## ? 우선순위 액션

### ?? 긴급 (지금 바로)
```
1. DevExpress API Key 재생성
2. 로컬 환경 업데이트
3. GitHub Secrets 업데이트
```

### ?? 중요 (오늘 중)
```
1. 빌드 및 테스트 확인
2. 팀원 알림
3. Git 히스토리 재작성 고려
```

### ?? 권장 (이번 주)
```
1. Pre-commit hook 설정
2. 보안 정책 문서화
3. 정기 검토 프로세스 수립
```

---

## ?? 지원

문제가 있거나 질문이 있다면:
- DevExpress Support: https://www.devexpress.com/support
- GitHub Security: https://github.com/security

---

**작성일**: 2024-01-10  
**심각도**: ?? 높음  
**상태**: ?? 조치 필요

---

**중요**: 이 문서를 읽은 후 즉시 DevExpress API Key를 재생성하세요!
