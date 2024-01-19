// automatically generated by the FlatBuffers compiler, do not modify


#ifndef FLATBUFFERS_GENERATED_QUERY_BOCCHITRACKER_PROCESSLINKQUERY_QUERIES_H_
#define FLATBUFFERS_GENERATED_QUERY_BOCCHITRACKER_PROCESSLINKQUERY_QUERIES_H_

#include "flatbuffers/flatbuffers.h"

// Ensure the included flatbuffers.h is the same version as when this file was
// generated, otherwise it may not be compatible.
static_assert(FLATBUFFERS_VERSION_MAJOR == 23 &&
              FLATBUFFERS_VERSION_MINOR == 5 &&
              FLATBUFFERS_VERSION_REVISION == 26,
             "Non-compatible flatbuffers version included");

namespace BocchiTracker {
namespace ProcessLinkQuery {
namespace Queries {

struct Vec3;

struct Issue;
struct IssueBuilder;

struct ScreenshotData;
struct ScreenshotDataBuilder;

struct PlayerPosition;
struct PlayerPositionBuilder;

struct AppBasicInfo;
struct AppBasicInfoBuilder;

struct ScreenshotRequest;
struct ScreenshotRequestBuilder;

struct LogData;
struct LogDataBuilder;

struct JumpRequest;
struct JumpRequestBuilder;

struct IssueesRequest;
struct IssueesRequestBuilder;

struct Packet;
struct PacketBuilder;

enum QueryID : uint8_t {
  QueryID_NONE = 0,
  QueryID_AppBasicInfo = 1,
  QueryID_PlayerPosition = 2,
  QueryID_ScreenshotData = 3,
  QueryID_LogData = 4,
  QueryID_ScreenshotRequest = 5,
  QueryID_JumpRequest = 6,
  QueryID_IssueesRequest = 7,
  QueryID_MIN = QueryID_NONE,
  QueryID_MAX = QueryID_IssueesRequest
};

inline const QueryID (&EnumValuesQueryID())[8] {
  static const QueryID values[] = {
    QueryID_NONE,
    QueryID_AppBasicInfo,
    QueryID_PlayerPosition,
    QueryID_ScreenshotData,
    QueryID_LogData,
    QueryID_ScreenshotRequest,
    QueryID_JumpRequest,
    QueryID_IssueesRequest
  };
  return values;
}

inline const char * const *EnumNamesQueryID() {
  static const char * const names[9] = {
    "NONE",
    "AppBasicInfo",
    "PlayerPosition",
    "ScreenshotData",
    "LogData",
    "ScreenshotRequest",
    "JumpRequest",
    "IssueesRequest",
    nullptr
  };
  return names;
}

inline const char *EnumNameQueryID(QueryID e) {
  if (::flatbuffers::IsOutRange(e, QueryID_NONE, QueryID_IssueesRequest)) return "";
  const size_t index = static_cast<size_t>(e);
  return EnumNamesQueryID()[index];
}

template<typename T> struct QueryIDTraits {
  static const QueryID enum_value = QueryID_NONE;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::AppBasicInfo> {
  static const QueryID enum_value = QueryID_AppBasicInfo;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::PlayerPosition> {
  static const QueryID enum_value = QueryID_PlayerPosition;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::ScreenshotData> {
  static const QueryID enum_value = QueryID_ScreenshotData;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::LogData> {
  static const QueryID enum_value = QueryID_LogData;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::ScreenshotRequest> {
  static const QueryID enum_value = QueryID_ScreenshotRequest;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::JumpRequest> {
  static const QueryID enum_value = QueryID_JumpRequest;
};

template<> struct QueryIDTraits<BocchiTracker::ProcessLinkQuery::Queries::IssueesRequest> {
  static const QueryID enum_value = QueryID_IssueesRequest;
};

bool VerifyQueryID(::flatbuffers::Verifier &verifier, const void *obj, QueryID type);
bool VerifyQueryIDVector(::flatbuffers::Verifier &verifier, const ::flatbuffers::Vector<::flatbuffers::Offset<void>> *values, const ::flatbuffers::Vector<uint8_t> *types);

FLATBUFFERS_MANUALLY_ALIGNED_STRUCT(4) Vec3 FLATBUFFERS_FINAL_CLASS {
 private:
  float x_;
  float y_;
  float z_;

 public:
  Vec3()
      : x_(0),
        y_(0),
        z_(0) {
  }
  Vec3(float _x, float _y, float _z)
      : x_(::flatbuffers::EndianScalar(_x)),
        y_(::flatbuffers::EndianScalar(_y)),
        z_(::flatbuffers::EndianScalar(_z)) {
  }
  float x() const {
    return ::flatbuffers::EndianScalar(x_);
  }
  float y() const {
    return ::flatbuffers::EndianScalar(y_);
  }
  float z() const {
    return ::flatbuffers::EndianScalar(z_);
  }
};
FLATBUFFERS_STRUCT_END(Vec3, 12);

struct Issue FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef IssueBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_ID = 4,
    VT_SUMMARY = 6,
    VT_ASSIGN = 8,
    VT_STATUS = 10,
    VT_STAGE = 12,
    VT_LOCATION = 14
  };
  const ::flatbuffers::String *id() const {
    return GetPointer<const ::flatbuffers::String *>(VT_ID);
  }
  const ::flatbuffers::String *summary() const {
    return GetPointer<const ::flatbuffers::String *>(VT_SUMMARY);
  }
  const ::flatbuffers::String *assign() const {
    return GetPointer<const ::flatbuffers::String *>(VT_ASSIGN);
  }
  const ::flatbuffers::String *status() const {
    return GetPointer<const ::flatbuffers::String *>(VT_STATUS);
  }
  const ::flatbuffers::String *stage() const {
    return GetPointer<const ::flatbuffers::String *>(VT_STAGE);
  }
  const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location() const {
    return GetStruct<const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *>(VT_LOCATION);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyOffset(verifier, VT_ID) &&
           verifier.VerifyString(id()) &&
           VerifyOffset(verifier, VT_SUMMARY) &&
           verifier.VerifyString(summary()) &&
           VerifyOffset(verifier, VT_ASSIGN) &&
           verifier.VerifyString(assign()) &&
           VerifyOffset(verifier, VT_STATUS) &&
           verifier.VerifyString(status()) &&
           VerifyOffset(verifier, VT_STAGE) &&
           verifier.VerifyString(stage()) &&
           VerifyField<BocchiTracker::ProcessLinkQuery::Queries::Vec3>(verifier, VT_LOCATION, 4) &&
           verifier.EndTable();
  }
};

struct IssueBuilder {
  typedef Issue Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_id(::flatbuffers::Offset<::flatbuffers::String> id) {
    fbb_.AddOffset(Issue::VT_ID, id);
  }
  void add_summary(::flatbuffers::Offset<::flatbuffers::String> summary) {
    fbb_.AddOffset(Issue::VT_SUMMARY, summary);
  }
  void add_assign(::flatbuffers::Offset<::flatbuffers::String> assign) {
    fbb_.AddOffset(Issue::VT_ASSIGN, assign);
  }
  void add_status(::flatbuffers::Offset<::flatbuffers::String> status) {
    fbb_.AddOffset(Issue::VT_STATUS, status);
  }
  void add_stage(::flatbuffers::Offset<::flatbuffers::String> stage) {
    fbb_.AddOffset(Issue::VT_STAGE, stage);
  }
  void add_location(const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location) {
    fbb_.AddStruct(Issue::VT_LOCATION, location);
  }
  explicit IssueBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<Issue> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<Issue>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<Issue> CreateIssue(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    ::flatbuffers::Offset<::flatbuffers::String> id = 0,
    ::flatbuffers::Offset<::flatbuffers::String> summary = 0,
    ::flatbuffers::Offset<::flatbuffers::String> assign = 0,
    ::flatbuffers::Offset<::flatbuffers::String> status = 0,
    ::flatbuffers::Offset<::flatbuffers::String> stage = 0,
    const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location = nullptr) {
  IssueBuilder builder_(_fbb);
  builder_.add_location(location);
  builder_.add_stage(stage);
  builder_.add_status(status);
  builder_.add_assign(assign);
  builder_.add_summary(summary);
  builder_.add_id(id);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<Issue> CreateIssueDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    const char *id = nullptr,
    const char *summary = nullptr,
    const char *assign = nullptr,
    const char *status = nullptr,
    const char *stage = nullptr,
    const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location = nullptr) {
  auto id__ = id ? _fbb.CreateString(id) : 0;
  auto summary__ = summary ? _fbb.CreateString(summary) : 0;
  auto assign__ = assign ? _fbb.CreateString(assign) : 0;
  auto status__ = status ? _fbb.CreateString(status) : 0;
  auto stage__ = stage ? _fbb.CreateString(stage) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreateIssue(
      _fbb,
      id__,
      summary__,
      assign__,
      status__,
      stage__,
      location);
}

struct ScreenshotData FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef ScreenshotDataBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_WIDTH = 4,
    VT_HEIGHT = 6,
    VT_DATA = 8
  };
  int32_t width() const {
    return GetField<int32_t>(VT_WIDTH, 0);
  }
  int32_t height() const {
    return GetField<int32_t>(VT_HEIGHT, 0);
  }
  const ::flatbuffers::Vector<uint8_t> *data() const {
    return GetPointer<const ::flatbuffers::Vector<uint8_t> *>(VT_DATA);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyField<int32_t>(verifier, VT_WIDTH, 4) &&
           VerifyField<int32_t>(verifier, VT_HEIGHT, 4) &&
           VerifyOffset(verifier, VT_DATA) &&
           verifier.VerifyVector(data()) &&
           verifier.EndTable();
  }
};

struct ScreenshotDataBuilder {
  typedef ScreenshotData Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_width(int32_t width) {
    fbb_.AddElement<int32_t>(ScreenshotData::VT_WIDTH, width, 0);
  }
  void add_height(int32_t height) {
    fbb_.AddElement<int32_t>(ScreenshotData::VT_HEIGHT, height, 0);
  }
  void add_data(::flatbuffers::Offset<::flatbuffers::Vector<uint8_t>> data) {
    fbb_.AddOffset(ScreenshotData::VT_DATA, data);
  }
  explicit ScreenshotDataBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<ScreenshotData> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<ScreenshotData>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<ScreenshotData> CreateScreenshotData(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    int32_t width = 0,
    int32_t height = 0,
    ::flatbuffers::Offset<::flatbuffers::Vector<uint8_t>> data = 0) {
  ScreenshotDataBuilder builder_(_fbb);
  builder_.add_data(data);
  builder_.add_height(height);
  builder_.add_width(width);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<ScreenshotData> CreateScreenshotDataDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    int32_t width = 0,
    int32_t height = 0,
    const std::vector<uint8_t> *data = nullptr) {
  auto data__ = data ? _fbb.CreateVector<uint8_t>(*data) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreateScreenshotData(
      _fbb,
      width,
      height,
      data__);
}

struct PlayerPosition FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef PlayerPositionBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_X = 4,
    VT_Y = 6,
    VT_Z = 8,
    VT_STAGE = 10
  };
  float x() const {
    return GetField<float>(VT_X, 0.0f);
  }
  float y() const {
    return GetField<float>(VT_Y, 0.0f);
  }
  float z() const {
    return GetField<float>(VT_Z, 0.0f);
  }
  const ::flatbuffers::String *stage() const {
    return GetPointer<const ::flatbuffers::String *>(VT_STAGE);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyField<float>(verifier, VT_X, 4) &&
           VerifyField<float>(verifier, VT_Y, 4) &&
           VerifyField<float>(verifier, VT_Z, 4) &&
           VerifyOffset(verifier, VT_STAGE) &&
           verifier.VerifyString(stage()) &&
           verifier.EndTable();
  }
};

struct PlayerPositionBuilder {
  typedef PlayerPosition Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_x(float x) {
    fbb_.AddElement<float>(PlayerPosition::VT_X, x, 0.0f);
  }
  void add_y(float y) {
    fbb_.AddElement<float>(PlayerPosition::VT_Y, y, 0.0f);
  }
  void add_z(float z) {
    fbb_.AddElement<float>(PlayerPosition::VT_Z, z, 0.0f);
  }
  void add_stage(::flatbuffers::Offset<::flatbuffers::String> stage) {
    fbb_.AddOffset(PlayerPosition::VT_STAGE, stage);
  }
  explicit PlayerPositionBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<PlayerPosition> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<PlayerPosition>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<PlayerPosition> CreatePlayerPosition(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    float x = 0.0f,
    float y = 0.0f,
    float z = 0.0f,
    ::flatbuffers::Offset<::flatbuffers::String> stage = 0) {
  PlayerPositionBuilder builder_(_fbb);
  builder_.add_stage(stage);
  builder_.add_z(z);
  builder_.add_y(y);
  builder_.add_x(x);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<PlayerPosition> CreatePlayerPositionDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    float x = 0.0f,
    float y = 0.0f,
    float z = 0.0f,
    const char *stage = nullptr) {
  auto stage__ = stage ? _fbb.CreateString(stage) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreatePlayerPosition(
      _fbb,
      x,
      y,
      z,
      stage__);
}

struct AppBasicInfo FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef AppBasicInfoBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_PID = 4,
    VT_APP_NAME = 6,
    VT_ARGS = 8,
    VT_PLATFORM = 10,
    VT_LOG_FILEPATH = 12
  };
  int32_t pid() const {
    return GetField<int32_t>(VT_PID, 0);
  }
  const ::flatbuffers::String *app_name() const {
    return GetPointer<const ::flatbuffers::String *>(VT_APP_NAME);
  }
  const ::flatbuffers::String *args() const {
    return GetPointer<const ::flatbuffers::String *>(VT_ARGS);
  }
  const ::flatbuffers::String *platform() const {
    return GetPointer<const ::flatbuffers::String *>(VT_PLATFORM);
  }
  const ::flatbuffers::String *log_filepath() const {
    return GetPointer<const ::flatbuffers::String *>(VT_LOG_FILEPATH);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyField<int32_t>(verifier, VT_PID, 4) &&
           VerifyOffset(verifier, VT_APP_NAME) &&
           verifier.VerifyString(app_name()) &&
           VerifyOffset(verifier, VT_ARGS) &&
           verifier.VerifyString(args()) &&
           VerifyOffset(verifier, VT_PLATFORM) &&
           verifier.VerifyString(platform()) &&
           VerifyOffset(verifier, VT_LOG_FILEPATH) &&
           verifier.VerifyString(log_filepath()) &&
           verifier.EndTable();
  }
};

struct AppBasicInfoBuilder {
  typedef AppBasicInfo Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_pid(int32_t pid) {
    fbb_.AddElement<int32_t>(AppBasicInfo::VT_PID, pid, 0);
  }
  void add_app_name(::flatbuffers::Offset<::flatbuffers::String> app_name) {
    fbb_.AddOffset(AppBasicInfo::VT_APP_NAME, app_name);
  }
  void add_args(::flatbuffers::Offset<::flatbuffers::String> args) {
    fbb_.AddOffset(AppBasicInfo::VT_ARGS, args);
  }
  void add_platform(::flatbuffers::Offset<::flatbuffers::String> platform) {
    fbb_.AddOffset(AppBasicInfo::VT_PLATFORM, platform);
  }
  void add_log_filepath(::flatbuffers::Offset<::flatbuffers::String> log_filepath) {
    fbb_.AddOffset(AppBasicInfo::VT_LOG_FILEPATH, log_filepath);
  }
  explicit AppBasicInfoBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<AppBasicInfo> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<AppBasicInfo>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<AppBasicInfo> CreateAppBasicInfo(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    int32_t pid = 0,
    ::flatbuffers::Offset<::flatbuffers::String> app_name = 0,
    ::flatbuffers::Offset<::flatbuffers::String> args = 0,
    ::flatbuffers::Offset<::flatbuffers::String> platform = 0,
    ::flatbuffers::Offset<::flatbuffers::String> log_filepath = 0) {
  AppBasicInfoBuilder builder_(_fbb);
  builder_.add_log_filepath(log_filepath);
  builder_.add_platform(platform);
  builder_.add_args(args);
  builder_.add_app_name(app_name);
  builder_.add_pid(pid);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<AppBasicInfo> CreateAppBasicInfoDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    int32_t pid = 0,
    const char *app_name = nullptr,
    const char *args = nullptr,
    const char *platform = nullptr,
    const char *log_filepath = nullptr) {
  auto app_name__ = app_name ? _fbb.CreateString(app_name) : 0;
  auto args__ = args ? _fbb.CreateString(args) : 0;
  auto platform__ = platform ? _fbb.CreateString(platform) : 0;
  auto log_filepath__ = log_filepath ? _fbb.CreateString(log_filepath) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreateAppBasicInfo(
      _fbb,
      pid,
      app_name__,
      args__,
      platform__,
      log_filepath__);
}

struct ScreenshotRequest FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef ScreenshotRequestBuilder Builder;
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           verifier.EndTable();
  }
};

struct ScreenshotRequestBuilder {
  typedef ScreenshotRequest Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  explicit ScreenshotRequestBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<ScreenshotRequest> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<ScreenshotRequest>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<ScreenshotRequest> CreateScreenshotRequest(
    ::flatbuffers::FlatBufferBuilder &_fbb) {
  ScreenshotRequestBuilder builder_(_fbb);
  return builder_.Finish();
}

struct LogData FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef LogDataBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_LOG = 4
  };
  const ::flatbuffers::String *log() const {
    return GetPointer<const ::flatbuffers::String *>(VT_LOG);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyOffset(verifier, VT_LOG) &&
           verifier.VerifyString(log()) &&
           verifier.EndTable();
  }
};

struct LogDataBuilder {
  typedef LogData Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_log(::flatbuffers::Offset<::flatbuffers::String> log) {
    fbb_.AddOffset(LogData::VT_LOG, log);
  }
  explicit LogDataBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<LogData> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<LogData>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<LogData> CreateLogData(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    ::flatbuffers::Offset<::flatbuffers::String> log = 0) {
  LogDataBuilder builder_(_fbb);
  builder_.add_log(log);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<LogData> CreateLogDataDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    const char *log = nullptr) {
  auto log__ = log ? _fbb.CreateString(log) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreateLogData(
      _fbb,
      log__);
}

struct JumpRequest FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef JumpRequestBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_LOCATION = 4,
    VT_STAGE = 6
  };
  const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location() const {
    return GetStruct<const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *>(VT_LOCATION);
  }
  const ::flatbuffers::String *stage() const {
    return GetPointer<const ::flatbuffers::String *>(VT_STAGE);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyField<BocchiTracker::ProcessLinkQuery::Queries::Vec3>(verifier, VT_LOCATION, 4) &&
           VerifyOffset(verifier, VT_STAGE) &&
           verifier.VerifyString(stage()) &&
           verifier.EndTable();
  }
};

struct JumpRequestBuilder {
  typedef JumpRequest Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_location(const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location) {
    fbb_.AddStruct(JumpRequest::VT_LOCATION, location);
  }
  void add_stage(::flatbuffers::Offset<::flatbuffers::String> stage) {
    fbb_.AddOffset(JumpRequest::VT_STAGE, stage);
  }
  explicit JumpRequestBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<JumpRequest> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<JumpRequest>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<JumpRequest> CreateJumpRequest(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location = nullptr,
    ::flatbuffers::Offset<::flatbuffers::String> stage = 0) {
  JumpRequestBuilder builder_(_fbb);
  builder_.add_stage(stage);
  builder_.add_location(location);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<JumpRequest> CreateJumpRequestDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    const BocchiTracker::ProcessLinkQuery::Queries::Vec3 *location = nullptr,
    const char *stage = nullptr) {
  auto stage__ = stage ? _fbb.CreateString(stage) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreateJumpRequest(
      _fbb,
      location,
      stage__);
}

struct IssueesRequest FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef IssueesRequestBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_ISSUES = 4
  };
  const ::flatbuffers::Vector<::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Issue>> *issues() const {
    return GetPointer<const ::flatbuffers::Vector<::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Issue>> *>(VT_ISSUES);
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyOffset(verifier, VT_ISSUES) &&
           verifier.VerifyVector(issues()) &&
           verifier.VerifyVectorOfTables(issues()) &&
           verifier.EndTable();
  }
};

struct IssueesRequestBuilder {
  typedef IssueesRequest Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_issues(::flatbuffers::Offset<::flatbuffers::Vector<::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Issue>>> issues) {
    fbb_.AddOffset(IssueesRequest::VT_ISSUES, issues);
  }
  explicit IssueesRequestBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<IssueesRequest> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<IssueesRequest>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<IssueesRequest> CreateIssueesRequest(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    ::flatbuffers::Offset<::flatbuffers::Vector<::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Issue>>> issues = 0) {
  IssueesRequestBuilder builder_(_fbb);
  builder_.add_issues(issues);
  return builder_.Finish();
}

inline ::flatbuffers::Offset<IssueesRequest> CreateIssueesRequestDirect(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    const std::vector<::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Issue>> *issues = nullptr) {
  auto issues__ = issues ? _fbb.CreateVector<::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Issue>>(*issues) : 0;
  return BocchiTracker::ProcessLinkQuery::Queries::CreateIssueesRequest(
      _fbb,
      issues__);
}

struct Packet FLATBUFFERS_FINAL_CLASS : private ::flatbuffers::Table {
  typedef PacketBuilder Builder;
  enum FlatBuffersVTableOffset FLATBUFFERS_VTABLE_UNDERLYING_TYPE {
    VT_QUERY_ID_TYPE = 4,
    VT_QUERY_ID = 6
  };
  BocchiTracker::ProcessLinkQuery::Queries::QueryID query_id_type() const {
    return static_cast<BocchiTracker::ProcessLinkQuery::Queries::QueryID>(GetField<uint8_t>(VT_QUERY_ID_TYPE, 0));
  }
  const void *query_id() const {
    return GetPointer<const void *>(VT_QUERY_ID);
  }
  template<typename T> const T *query_id_as() const;
  const BocchiTracker::ProcessLinkQuery::Queries::AppBasicInfo *query_id_as_AppBasicInfo() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_AppBasicInfo ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::AppBasicInfo *>(query_id()) : nullptr;
  }
  const BocchiTracker::ProcessLinkQuery::Queries::PlayerPosition *query_id_as_PlayerPosition() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_PlayerPosition ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::PlayerPosition *>(query_id()) : nullptr;
  }
  const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotData *query_id_as_ScreenshotData() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_ScreenshotData ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotData *>(query_id()) : nullptr;
  }
  const BocchiTracker::ProcessLinkQuery::Queries::LogData *query_id_as_LogData() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_LogData ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::LogData *>(query_id()) : nullptr;
  }
  const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotRequest *query_id_as_ScreenshotRequest() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_ScreenshotRequest ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotRequest *>(query_id()) : nullptr;
  }
  const BocchiTracker::ProcessLinkQuery::Queries::JumpRequest *query_id_as_JumpRequest() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_JumpRequest ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::JumpRequest *>(query_id()) : nullptr;
  }
  const BocchiTracker::ProcessLinkQuery::Queries::IssueesRequest *query_id_as_IssueesRequest() const {
    return query_id_type() == BocchiTracker::ProcessLinkQuery::Queries::QueryID_IssueesRequest ? static_cast<const BocchiTracker::ProcessLinkQuery::Queries::IssueesRequest *>(query_id()) : nullptr;
  }
  bool Verify(::flatbuffers::Verifier &verifier) const {
    return VerifyTableStart(verifier) &&
           VerifyField<uint8_t>(verifier, VT_QUERY_ID_TYPE, 1) &&
           VerifyOffset(verifier, VT_QUERY_ID) &&
           VerifyQueryID(verifier, query_id(), query_id_type()) &&
           verifier.EndTable();
  }
};

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::AppBasicInfo *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::AppBasicInfo>() const {
  return query_id_as_AppBasicInfo();
}

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::PlayerPosition *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::PlayerPosition>() const {
  return query_id_as_PlayerPosition();
}

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotData *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::ScreenshotData>() const {
  return query_id_as_ScreenshotData();
}

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::LogData *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::LogData>() const {
  return query_id_as_LogData();
}

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotRequest *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::ScreenshotRequest>() const {
  return query_id_as_ScreenshotRequest();
}

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::JumpRequest *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::JumpRequest>() const {
  return query_id_as_JumpRequest();
}

template<> inline const BocchiTracker::ProcessLinkQuery::Queries::IssueesRequest *Packet::query_id_as<BocchiTracker::ProcessLinkQuery::Queries::IssueesRequest>() const {
  return query_id_as_IssueesRequest();
}

struct PacketBuilder {
  typedef Packet Table;
  ::flatbuffers::FlatBufferBuilder &fbb_;
  ::flatbuffers::uoffset_t start_;
  void add_query_id_type(BocchiTracker::ProcessLinkQuery::Queries::QueryID query_id_type) {
    fbb_.AddElement<uint8_t>(Packet::VT_QUERY_ID_TYPE, static_cast<uint8_t>(query_id_type), 0);
  }
  void add_query_id(::flatbuffers::Offset<void> query_id) {
    fbb_.AddOffset(Packet::VT_QUERY_ID, query_id);
  }
  explicit PacketBuilder(::flatbuffers::FlatBufferBuilder &_fbb)
        : fbb_(_fbb) {
    start_ = fbb_.StartTable();
  }
  ::flatbuffers::Offset<Packet> Finish() {
    const auto end = fbb_.EndTable(start_);
    auto o = ::flatbuffers::Offset<Packet>(end);
    return o;
  }
};

inline ::flatbuffers::Offset<Packet> CreatePacket(
    ::flatbuffers::FlatBufferBuilder &_fbb,
    BocchiTracker::ProcessLinkQuery::Queries::QueryID query_id_type = BocchiTracker::ProcessLinkQuery::Queries::QueryID_NONE,
    ::flatbuffers::Offset<void> query_id = 0) {
  PacketBuilder builder_(_fbb);
  builder_.add_query_id(query_id);
  builder_.add_query_id_type(query_id_type);
  return builder_.Finish();
}

inline bool VerifyQueryID(::flatbuffers::Verifier &verifier, const void *obj, QueryID type) {
  switch (type) {
    case QueryID_NONE: {
      return true;
    }
    case QueryID_AppBasicInfo: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::AppBasicInfo *>(obj);
      return verifier.VerifyTable(ptr);
    }
    case QueryID_PlayerPosition: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::PlayerPosition *>(obj);
      return verifier.VerifyTable(ptr);
    }
    case QueryID_ScreenshotData: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotData *>(obj);
      return verifier.VerifyTable(ptr);
    }
    case QueryID_LogData: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::LogData *>(obj);
      return verifier.VerifyTable(ptr);
    }
    case QueryID_ScreenshotRequest: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::ScreenshotRequest *>(obj);
      return verifier.VerifyTable(ptr);
    }
    case QueryID_JumpRequest: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::JumpRequest *>(obj);
      return verifier.VerifyTable(ptr);
    }
    case QueryID_IssueesRequest: {
      auto ptr = reinterpret_cast<const BocchiTracker::ProcessLinkQuery::Queries::IssueesRequest *>(obj);
      return verifier.VerifyTable(ptr);
    }
    default: return true;
  }
}

inline bool VerifyQueryIDVector(::flatbuffers::Verifier &verifier, const ::flatbuffers::Vector<::flatbuffers::Offset<void>> *values, const ::flatbuffers::Vector<uint8_t> *types) {
  if (!values || !types) return !values && !types;
  if (values->size() != types->size()) return false;
  for (::flatbuffers::uoffset_t i = 0; i < values->size(); ++i) {
    if (!VerifyQueryID(
        verifier,  values->Get(i), types->GetEnum<QueryID>(i))) {
      return false;
    }
  }
  return true;
}

inline const BocchiTracker::ProcessLinkQuery::Queries::Packet *GetPacket(const void *buf) {
  return ::flatbuffers::GetRoot<BocchiTracker::ProcessLinkQuery::Queries::Packet>(buf);
}

inline const BocchiTracker::ProcessLinkQuery::Queries::Packet *GetSizePrefixedPacket(const void *buf) {
  return ::flatbuffers::GetSizePrefixedRoot<BocchiTracker::ProcessLinkQuery::Queries::Packet>(buf);
}

inline bool VerifyPacketBuffer(
    ::flatbuffers::Verifier &verifier) {
  return verifier.VerifyBuffer<BocchiTracker::ProcessLinkQuery::Queries::Packet>(nullptr);
}

inline bool VerifySizePrefixedPacketBuffer(
    ::flatbuffers::Verifier &verifier) {
  return verifier.VerifySizePrefixedBuffer<BocchiTracker::ProcessLinkQuery::Queries::Packet>(nullptr);
}

inline void FinishPacketBuffer(
    ::flatbuffers::FlatBufferBuilder &fbb,
    ::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Packet> root) {
  fbb.Finish(root);
}

inline void FinishSizePrefixedPacketBuffer(
    ::flatbuffers::FlatBufferBuilder &fbb,
    ::flatbuffers::Offset<BocchiTracker::ProcessLinkQuery::Queries::Packet> root) {
  fbb.FinishSizePrefixed(root);
}

}  // namespace Queries
}  // namespace ProcessLinkQuery
}  // namespace BocchiTracker

#endif  // FLATBUFFERS_GENERATED_QUERY_BOCCHITRACKER_PROCESSLINKQUERY_QUERIES_H_
