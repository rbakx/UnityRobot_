// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: google/protobuf/unittest_no_arena_import.proto

#ifndef PROTOBUF_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto__INCLUDED
#define PROTOBUF_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto__INCLUDED

#include <string>

#include <google/protobuf/stubs/common.h>

#if GOOGLE_PROTOBUF_VERSION < 3000000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please update
#error your headers.
#endif
#if 3000002 < GOOGLE_PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers.  Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/metadata.h>
#include <google/protobuf/message.h>
#include <google/protobuf/repeated_field.h>
#include <google/protobuf/extension_set.h>
#include <google/protobuf/unknown_field_set.h>
// @@protoc_insertion_point(includes)

namespace proto2_arena_unittest {

// Internal implementation detail -- do not call these.
void protobuf_AddDesc_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto();
void protobuf_AssignDesc_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto();
void protobuf_ShutdownFile_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto();

class ImportNoArenaNestedMessage;

// ===================================================================

class ImportNoArenaNestedMessage : public ::google::protobuf::Message /* @@protoc_insertion_point(class_definition:proto2_arena_unittest.ImportNoArenaNestedMessage) */ {
 public:
  ImportNoArenaNestedMessage();
  virtual ~ImportNoArenaNestedMessage();

  ImportNoArenaNestedMessage(const ImportNoArenaNestedMessage& from);

  inline ImportNoArenaNestedMessage& operator=(const ImportNoArenaNestedMessage& from) {
    CopyFrom(from);
    return *this;
  }

  inline const ::google::protobuf::UnknownFieldSet& unknown_fields() const {
    return _internal_metadata_.unknown_fields();
  }

  inline ::google::protobuf::UnknownFieldSet* mutable_unknown_fields() {
    return _internal_metadata_.mutable_unknown_fields();
  }

  static const ::google::protobuf::Descriptor* descriptor();
  static const ImportNoArenaNestedMessage& default_instance();

  void Swap(ImportNoArenaNestedMessage* other);

  // implements Message ----------------------------------------------

  inline ImportNoArenaNestedMessage* New() const { return New(NULL); }

  ImportNoArenaNestedMessage* New(::google::protobuf::Arena* arena) const;
  void CopyFrom(const ::google::protobuf::Message& from);
  void MergeFrom(const ::google::protobuf::Message& from);
  void CopyFrom(const ImportNoArenaNestedMessage& from);
  void MergeFrom(const ImportNoArenaNestedMessage& from);
  void Clear();
  bool IsInitialized() const;

  int ByteSize() const;
  bool MergePartialFromCodedStream(
      ::google::protobuf::io::CodedInputStream* input);
  void SerializeWithCachedSizes(
      ::google::protobuf::io::CodedOutputStream* output) const;
  ::google::protobuf::uint8* InternalSerializeWithCachedSizesToArray(
      bool deterministic, ::google::protobuf::uint8* output) const;
  ::google::protobuf::uint8* SerializeWithCachedSizesToArray(::google::protobuf::uint8* output) const {
    return InternalSerializeWithCachedSizesToArray(false, output);
  }
  int GetCachedSize() const { return _cached_size_; }
  private:
  void SharedCtor();
  void SharedDtor();
  void SetCachedSize(int size) const;
  void InternalSwap(ImportNoArenaNestedMessage* other);
  private:
  inline ::google::protobuf::Arena* GetArenaNoVirtual() const {
    return _internal_metadata_.arena();
  }
  inline void* MaybeArenaPtr() const {
    return _internal_metadata_.raw_arena_ptr();
  }
  public:

  ::google::protobuf::Metadata GetMetadata() const;

  // nested types ----------------------------------------------------

  // accessors -------------------------------------------------------

  // optional int32 d = 1;
  bool has_d() const;
  void clear_d();
  static const int kDFieldNumber = 1;
  ::google::protobuf::int32 d() const;
  void set_d(::google::protobuf::int32 value);

  // @@protoc_insertion_point(class_scope:proto2_arena_unittest.ImportNoArenaNestedMessage)
 private:
  inline void set_has_d();
  inline void clear_has_d();

  ::google::protobuf::internal::InternalMetadataWithArena _internal_metadata_;
  ::google::protobuf::uint32 _has_bits_[1];
  mutable int _cached_size_;
  ::google::protobuf::int32 d_;
  friend void  protobuf_AddDesc_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto();
  friend void protobuf_AssignDesc_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto();
  friend void protobuf_ShutdownFile_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto();

  void InitAsDefaultInstance();
  static ImportNoArenaNestedMessage* default_instance_;
};
// ===================================================================


// ===================================================================

#if !PROTOBUF_INLINE_NOT_IN_HEADERS
// ImportNoArenaNestedMessage

// optional int32 d = 1;
inline bool ImportNoArenaNestedMessage::has_d() const {
  return (_has_bits_[0] & 0x00000001u) != 0;
}
inline void ImportNoArenaNestedMessage::set_has_d() {
  _has_bits_[0] |= 0x00000001u;
}
inline void ImportNoArenaNestedMessage::clear_has_d() {
  _has_bits_[0] &= ~0x00000001u;
}
inline void ImportNoArenaNestedMessage::clear_d() {
  d_ = 0;
  clear_has_d();
}
inline ::google::protobuf::int32 ImportNoArenaNestedMessage::d() const {
  // @@protoc_insertion_point(field_get:proto2_arena_unittest.ImportNoArenaNestedMessage.d)
  return d_;
}
inline void ImportNoArenaNestedMessage::set_d(::google::protobuf::int32 value) {
  set_has_d();
  d_ = value;
  // @@protoc_insertion_point(field_set:proto2_arena_unittest.ImportNoArenaNestedMessage.d)
}

#endif  // !PROTOBUF_INLINE_NOT_IN_HEADERS

// @@protoc_insertion_point(namespace_scope)

}  // namespace proto2_arena_unittest

// @@protoc_insertion_point(global_scope)

#endif  // PROTOBUF_google_2fprotobuf_2funittest_5fno_5farena_5fimport_2eproto__INCLUDED