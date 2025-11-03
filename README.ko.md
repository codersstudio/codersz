# Coders: JSSP 트랜스파일러 매뉴얼 (한국어)

**Coders**는 **JSSP**라는 단일 통합 언어로 작성된 코드를 다양한 플랫폼의 네이티브 코드로 변환하는 강력한 트랜스파일러입니다. 이 매뉴얼은 Spring Boot 백엔드와 Vue.js 프런트엔드를 포함한 풀스택 개발에서 Coders를 사용하는 방법을 자세히 안내합니다.

## 설치

Coders는 NuGet.org를 통해 .NET 도구로 배포됩니다. 다음 명령으로 전역 설치할 수 있습니다:

```sh
dotnet tool install -g coders
```

## 빠른 시작

1. **워크스페이스 부트스트랩**
   ```sh
   coders init
   ```
   이 명령은 `config.yml`, 시작용 `main.jssp`, 그리고 예제 보조 파일(`property.jssp`, `schema.jssp`, `mapper.jssp`)을 생성합니다. 파일을 다시 생성하려면 `--force`를 추가합니다.

2. **생성된 구성 살펴보기**
   기본 `config.yml`에는 자주 사용하는 플랫폼을 위한 빌드 대상이 미리 정의되어 있습니다:
   - `cpp`, `java`, `kotlin`, `csharp`
   - `javascript`, `typescript`, `nodejs`, `nodets`, `reactjs`, `vuejs`, `svelte`, `flutter`
   - `python`, `django`, `go`, `goserver`, `rust`, `rustserver`
   - `springboot`
   프로젝트에 맞게 `outPath`, `entry`, `options` 값을 조정하십시오.

3. **대상 빌드**
   ```sh
   coders build -p vuejs
   coders build -p springboot -v
   ```
   `-p|--projectId`로 `config.yml`에 정의된 프로젝트를 선택합니다. 다른 구성 파일을 사용하려면 `-c|--config`, LLM 빌더에 위임하려면 `-e|--engine llm`, 상세 로그를 보려면 `-v`를 추가합니다.

## CLI 참조

- `coders init [-f|--force]` – `config.yml`과 시작용 JSSP 자산을 스캐폴딩합니다. 깨끗한 상태가 필요할 때 `--force`로 다시 실행해도 안전합니다.
- `coders build -p <projectId> [-c <config.yml>] [-e builtin|llm] [-v]` – 지정한 JSSP 엔트리 파일을 파싱하고 각 프로젝트의 `outPath`에 플랫폼별 코드를 생성합니다. `llmOptions`가 정의되어 있고 `--engine llm`이 선택되면 Coders는 해당 LLM으로 빌드 작업을 위임합니다. `-p`를 생략하면 솔루션 엔트리 포인트에 대한 문법 검사만 수행합니다.

`--engine` 플래그는 현재 `builtin`과 `llm` 두 가지 모드를 지원합니다. `builtin`은 Coders의 네이티브 파서와 코드 생성기에 의존하므로 아직 일부 플랫폼만 제공되며, 향후 지속적으로 확장될 예정입니다. `llm`은 설정된 LLM 백엔드에 번역을 위임해 더 넓은 언어 범위를 처리합니다.

---

## 파트 1: 핵심 JSSP 개념

JSSP는 친숙한 인기 언어 문법을 차용하지만, 진정한 강점은 특화된 클래스 타입에 있습니다. 결국 모든 타입은 대상 언어의 클래스로 변환되지만, JSSP에서 타입을 지정하면 Coders가 플랫폼에 최적화된 정확하고 풍부한 기능의 코드를 생성할 수 있습니다.

- **변수**: `var <name> <type> = <value>;`
- **함수**: `func <name>(<args>) <return_type> { ... }`

### JSSP 클래스 타입

JSSP에서 사용할 수 있는 주요 클래스 타입은 다음과 같습니다:

- `class`: 범용 표준 클래스입니다.
- `struct`: DTO나 단순 객체와 비슷한 데이터 저장용 구조체입니다.
- `controller`: 엔드포인트를 노출하는 백엔드 API 컨트롤러를 정의합니다.
- `service`: 비즈니스 로직과 트랜잭션을 처리하는 백엔드 서비스를 정의합니다.
- `mapper`: 데이터베이스 쿼리를 정의하고 실행하는 데이터 접근 클래스를 정의합니다.
- `html`: 템플릿, 스크립트, 스타일을 포함한 프런트엔드 컴포넌트를 정의합니다.
- `widget`: Dart 언어로 Android와 iOS 앱을 개발하기 위한 Flutter 위젯 클래스를 정의합니다.

### JSSP의 데이터 타입

JSSP는 일반 프로그래밍 로직에 사용하는 데이터 타입과 데이터베이스 상호작용에 사용하는 데이터 타입을 구분합니다.

- **프로그래밍 언어 타입**: `class`, `service`, `controller` 등에서 사용하며 Java, C++, C# 등의 네이티브 타입으로 변환됩니다. 예: `int32`, `int64`, `float`, `double`, `string`, `bool`, `list<T>`, `map<K,V>`.
- **데이터베이스 & 매퍼 타입**: `mapper`와 `query` 블록에서만 사용되며 표준 SQL 데이터 타입에 대응합니다. 예: `int`, `varchar(50)`, `char(10)`, `text`, `datetime`.

---

## 파트 2: Spring Boot로 백엔드 개발

Coders는 JSSP 소스에서 데이터 접근 계층과 REST 컨트롤러를 포함한 완전한 Spring Boot 애플리케이션을 생성할 수 있습니다.

### 2.1: 매퍼로 시작하는 데이터베이스 우선 접근

`.jssp` 파일에서 데이터베이스 스키마, 엔티티, 쿼리를 정의합니다.

- `table`: 데이터베이스 테이블을 정의합니다.
- `entity`: 쿼리 결과에 매핑되는 Java 클래스(DTO/VO)를 정의합니다.
- `mapper`: DAO(Data Access Object) 인터페이스를 정의합니다.
- `query`: 매퍼 안에서 SQL 쿼리를 정의합니다. 매개변수는 `:`로 시작합니다.

**예시: `user_mapper.jssp`**
```jssp
// 테이블 구조 정의
table tb_user {
  email varchar(32);
  nickname varchar(128);
  age int;
  key(email);
}

// 쿼리 결과를 받을 엔티티 정의
entity UserVo {
    var email varchar(32);
    var nickname varchar(128);
    var age int;
}

// 매퍼와 그 안의 쿼리 정의
mapper UserMapper {
    query selectUserByEmail(email varchar(32)) UserVo {
        select email, nickname, age
        from tb_user
        where email = :email;
    }

    query countUsers() int {
        select count(*) from tb_user;
    }
}
```

Coders는 이를 Spring Boot 애플리케이션에서 바로 사용할 수 있는 `UserMapper.java` 인터페이스와 MyBatis용 XML 파일로 변환합니다.

### 2.2: REST API 컨트롤러 만들기

`controller` 키워드로 Spring Boot REST 컨트롤러를 정의합니다.

- `controller`: `@RestController`로 변환될 클래스를 정의합니다.
- `[baseUrl]`: 컨트롤러의 기본 요청 경로를 설정합니다.
- `[method]`, `[action]`: 함수를 요청 처리기로 지정하는 속성입니다(예: `@GetMapping`, `@PostMapping`).
- **의존성 주입**: 매퍼를 사용하려면 `@mapper` 접두어로 메서드를 호출하기만 하면 됩니다. Coders가 생성된 Spring Boot 컨트롤러에서 필요한 의존성을 자동으로 주입합니다.

**예시: `user_controller.jssp`**
```jssp
// 앞에서 정의한 매퍼 가져오기
import "user_mapper.jssp"

[baseUrl='/api/v1/users']
controller UserController {

    // /api/v1/users/count GET 엔드포인트로 변환됩니다.
    [method=get, route='count']
    handler getUserCount() int {
        // 매퍼 함수를 호출합니다. Coders가 의존성 주입을 처리합니다.
        return @mapper.UserMapper.countUsers();
    }

    // /api/v1/users/{email} GET 엔드포인트로 변환됩니다.
    [method=get, route='{email}']
    handler getUserByEmail(email string) UserVo {
        return @mapper.UserMapper.selectUserByEmail(email);
    }
}
```

Coders는 이를 `@RestController`, `@RequestMapping`, `@Autowired` 주석이 올바르게 연결된 `UserController.java`로 변환합니다.

---

## 파트 3: 프런트엔드 개발

Coders는 JSSP로 컴포넌트를 정의하여 최신 프런트엔드 프레임워크 코드를 생성합니다. 현재 **Vue.js**와 **React.js**가 지원 대상입니다.

Coders는 JSSP 안에서 Vue.js나 React.js의 전체 문법을 지원하지 않습니다. 대신 `v-if`, `v-for`, `v-model`과 같은 HTML 조작 문법을 차용하여 익숙하고 강력한 형태로 제공하고, 선택한 프레임워크에 맞는 네이티브 코드로 변환합니다.

### 3.1: 컴포넌트 정의하기

`html` 키워드로 컴포넌트를 정의합니다. 파일에는 `<template>`, `<script>`, `<style>` 블록을 포함할 수 있으며, Coders는 이 단일 파일을 대상 프레임워크에 맞는 표준 컴포넌트(예: `.vue`, `.jsx`)로 변환합니다.

- `html`: 컴포넌트를 정의합니다.
- `[path]`: 출력 디렉터리와 파일명을 지정하는 속성입니다.
- **HTML 문법**: `<template>` 블록 안에서 `v-if`, `v-for`, `{{...}}`, `@click` 등 Vue 스타일의 디렉티브를 사용할 수 있으며 Coders가 이를 Vue나 React에 맞게 변환합니다.

**예시: `HomePage.jssp`**
```jssp
[path="views/HomePage"]
html HomePage {
    <template>
        <div>
            <h1 v-if="title">{{ title }}</h1>
            <input v-model="name" placeholder="이름을 입력하세요" />
            <button @click="greet">Greet</button>
        </div>
    </template>

    <script>
        var title string = "Welcome to Coders!";
        var name string = "";

        func greet() {
            @console.log("Hello, " + name);
        }
    </script>

    <style scoped>
        h1 {
            color: blue;
        }
    </style>
}
```

### 3.2: 고급 기능

JSSP는 프레임워크 네이티브 구현으로 변환되는 개념을 지원합니다.

- **Props**: `prop myProp string;`
- **Computed**: `computed myComputed string { ... }`
- **Emits**: `emit 'myEvent';`
- **클라이언트 측 저장소**: `store` 키워드는 브라우저의 `localStorage`를 활용하는 간단한 영속성 계층을 정의합니다.
- **라우팅**: `html` 블록에서 `[route, parent, redirect]`와 같은 속성을 사용해 라우팅 구성을 생성할 수 있습니다.

---

## 파트 4: 프런트엔드에서 타입 안전한 API 호출

Coders의 가장 강력한 기능 중 하나는 프런트엔드 JSSP 코드에서 백엔드 API를 완전한 타입 안전성을 유지하며 호출할 수 있다는 점입니다. 이를 통해 추측이나 런타임 오류를 제거할 수 있습니다.

백엔드용으로 정의된 모든 `controller`는 프런트엔드 JSSP 컨텍스트에서 자동으로 사용할 수 있습니다. Coders는 프런트엔드 코드(예: Vue 컴포넌트의 스크립트)에서 컨트롤러 메서드를 직접 호출할 수 있도록 `@api` 전용 문법을 제공합니다.

- **문법**: `@api.<ControllerName>.<methodName>(<args>)`
- **동작 원리**: Coders가 이 문법을 프런트엔드 파일에서 발견하면 대상 플랫폼에 맞는 비동기 HTTP 요청 코드로 자동 변환합니다. Vue.js에서는 `axios` 호출로 변환됩니다.
- **1:1 매핑**: 백엔드 `controller UserController`는 클라이언트 측 `ApiUserController` 클래스로 매핑되며 `@api` 문법을 통해 호출됩니다.

**예시: `HomePage.jssp`에서 API 호출**

파트 2에서 정의한 `UserController`에서 데이터를 가져오도록 Vue 컴포넌트의 `<script>` 블록을 수정합니다.

```jssp
// HomePage.jssp

<script>
    import { onMounted } from 'vue';

    var title string = "Welcome to Coders!";
    var userCount int = 0;

    // 컴포넌트가 생성될 때 API를 호출하기 위해 onMounted 훅 사용
    onMounted(async () => {
        // 직접 메서드를 호출하는 것처럼 보이지만 Coders가 API 호출로 변환합니다!
        userCount.value = await @api.UserController.getUserCount();
        title.value = "Total Users: " + userCount.value;
    });
</script>
```

**백엔드 내부 동작**

`await @api.UserController.getUserCount()` 줄은 Vue 컴포넌트 내부에서 다음 JavaScript 코드로 변환됩니다.

```javascript
// 생성된 JavaScript
import api from '@/api/ApiUserController'; // 생성된 API 클라이언트를 가정

// ... onMounted 훅 내부 ...
userCount.value = await api.getUserCount();
```

이 방식은 마치 로컬 함수를 호출하는 것처럼 프런트엔드와 백엔드 사이를 타입 안전하게 연결합니다.

---

## 파트 6: 보편적 번역을 위한 LLM 통합

Coders는 대형 언어 모델(LLM)과 통합해 기본 제공 트랜스파일러의 한계를 넘어설 수 있습니다. 이를 통해 LLM이 지원하는 거의 모든 프로그래밍 언어로 JSSP 코드를 번역할 수 있습니다.

프로덕션 환경에서 Coders는 Ollama에서 제공하는 `gpt-oss:20b` 모델과 OpenAI ChatGPT `gpt-4o` 모델을 기본 백엔드로 운영합니다. 이 기본값 덕분에 폭넓은 플랫폼을 포괄하면서 로컬 폴백도 유지할 수 있습니다.

이 기능을 활성화하려면 `config.yml` 파일에 `llmOptions` 블록을 추가합니다.

```yaml
# config.yml
llmOptions:
  provider: "ollama" # LLM 제공자(예: 'ollama', 'gemini', 'chatgpt')
  model: "codegemma:7b-instruct-v1.1-q8_0" # 사용할 구체적인 모델
  url: "http://localhost:11434" # LLM 서비스 호스트 URL
  apiKey: "" # 필요하다면 API 키

projects:
  # Coders가 기본적으로 지원하지 않는 플랫폼도 정의할 수 있습니다.
  - name: "golang project"
    platform: "go" # LLM이 Go 번역을 처리하도록 위임
    outPath: "./output/go"
    entry: "main.jssp"
```

Coders가 기본적으로 지원하지 않는 플랫폼(예: 예시의 `go`)을 정의하고 유효한 `llmOptions`를 제공하면, Coders는 JSSP 코드를 지정한 LLM에 전달해 목표 언어로 번역하도록 지시합니다. 이를 통해 Coders는 확장성과 미래 대응력이 뛰어난 트랜스파일 엔진이 됩니다.

---

## 로드맵: Coders의 미래

Coders는 빠르게 발전하고 있으며 현재 집중하고 있는 작업은 다음과 같습니다.

- 프로젝트 스캐폴딩(환경 설정, 샘플 데이터, 엔드 투 엔드 데모) 자동화 범위 확대
- 증분 빌드와 CLI에서 더욱 풍부한 진단을 제공하는 빠른 피드백 루프
- 더 많은 현실 세계 스택을 다루는 1차 템플릿과 문서 추가

이러한 개선 사항이 릴리스되면 공지 사항을 통해 확인할 수 있습니다.

---

## 파트 5: 풀스택 빌드 구성

`config.yml` 파일로 모든 빌드 대상을 관리합니다.

```yaml
# config.yml
projects:
  # 1. Spring Boot 백엔드 빌드
  - name: "backend"
    platform: "springboot"
    outPath: "./output/backend"
    entry:
      - "user_mapper.jssp"
      - "user_controller.jssp"
    options:
      package: "com.example.coders"

  # 2. Vue.js 프런트엔드 빌드
  - name: "frontend"
    platform: "vuejs"
    outPath: "./output/frontend/src"
    entry:
      - "HomePage.jssp"
      - "user_store.jssp"

  # 3. 프런트엔드용 API 클라이언트 생성
  - name: "api-client"
    platform: "javascript" # 또는 typescript
    outPath: "./output/frontend/src/api"
    entry: "user_controller.jssp"
    options:
      scriptMode: "ControllerApi" # 핵심 설정
```

`coders.exe`를 실행하면 단일하고 일관된 JSSP 코드베이스에서 전체 풀스택 애플리케이션을 트랜스파일할 수 있습니다.
