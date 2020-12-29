import React, { useEffect, useState } from 'react';
import { Breadcrumb, Button, Card, Col, Input, Layout, Row, Switch, Table, List, Tabs, Space, Badge, Modal, Collapse, Divider, Typography, Tag, Skeleton, Spin } from 'antd';
import { FileExcelOutlined } from '@ant-design/icons';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import moment from 'moment';

const Livestream = () => {
    const CustomTabPane = ({ key, tab, columns, loading, dataSource, videoId, ...props }) => {
        if (dataSource !== undefined) {
            return <Tabs.TabPane {...props}>
                <Row justify="center">
                    <Col xs={24} lg={24}>
                        <Space
                            style={{ width: "100%" }}
                            direction="vertical"
                        >
                            <Button
                                icon={<FileExcelOutlined />}
                                onClick={() => {
                                    setRootLoading(true);
                                    HttpGetExportExcel(videoId)
                                        .then(data => { })
                                        .catch(err => toast.error("Lỗi xuất file excel."));
                                }}
                            >Xuất Excel</Button>
                            <Card>
                                <Table
                                    onExpand={(expanded, record) => {
                                        if (expanded) {
                                            setExpandKey([record.key]);
                                        }
                                        else {
                                            setExpandKey([]);
                                        }
                                    }}
                                    expandedRowKeys={expandKey}
                                    style={{ textAlign: "center" }}
                                    loading={loading}
                                    dataSource={dataSource}
                                    columns={columns}
                                    bordered
                                    pagination={{ defaultPageSize: 10 }}
                                    scroll={{ x: 200 }}
                                />
                            </Card>
                        </Space>
                    </Col>
                </Row>
            </Tabs.TabPane>
        }
        else {
            return <Tabs.TabPane {...props}>
                <Row justify="center">
                    <Col xs={24} lg={24}>
                        <Card>
                            <Table
                                style={{ textAlign: "center" }}
                                loading={loading}
                                columns={columns}
                                bordered
                                pagination={{ defaultPageSize: 10 }}
                                scroll={{ x: 200 }}
                            />
                        </Card>
                    </Col>
                </Row>
            </Tabs.TabPane>
        }
    }

    const pageId = localStorage.getItem("pageId");
    const token = localStorage.getItem("access-token");

    const [expandKey, setExpandKey] = useState([0]);
    const [modalVisiable, setModalVisiable] = useState(false);
    const [videoLoading, setVideoLoading] = useState(true);
    const [commnetLoading, setCommentLoading] = useState(false);
    const [addPaneLoading, setAddPaneLoading] = useState(false);
    const [rootLoading, setRootLoading] = useState(false);
    const [videos, setVideos] = useState([]);
    const [comments, setComments] = useState([]);
    const [panes, setPanes] = useState([]);
    const [activePane, setActivePane] = useState("0");
    const [eventSources, setEventSources] = useState([]);
    const [modelData, setModelData] = useState({});

    //#region Dispose
    const disposeEventSource = (id) => {
        // Close eventSource
        let eventSource = eventSources.find(e => e.id === id);
        if (eventSource) {
            eventSource.instance.close();

            // Remove EventSource
            let newEventSources = eventSources.filter(e => e.id !== id);
            setEventSources(newEventSources);
        }
    }

    const disposeComment = (id) => {
        const newComments = comments.filter(cmt => cmt.id !== id);
        setComments(newComments);
    }

    const disposePane = (key) => {
        const newPanes = panes.filter(pane => pane.key !== key);
        setPanes(newPanes);
    }
    //#endregion

    useEffect(() => {
        console.log(`comments: ${comments.length} | eventSource: ${eventSources.length}`);
    }, []);

    useEffect(() => {
        HttpGetVideo(pageId).then(data => {
            setVideos(data);
            setVideoLoading(false);
        }).catch(err => {
            setVideoLoading(false);
        });

        return () => {
            eventSources.forEach(e => {
                e.instance.close();
            });
        }
    }, []);

    const columns = [
        {
            key: 1,
            title: "Tên",
            dataIndex: "FbName",
            width: 350,
            render: (value, row) => {
                // return row && row.children ?
                //     (
                //         <Badge size="small"
                //             count={row.children.length}
                //             offset={[25, 0]}
                //             style={{ backgroundColor: 'blue' }}>
                //             <a href={`https://fb.com/${row.FbId}`}>{value}</a>
                //         </Badge>
                //     ) : <a href={`https://fb.com/${row.FbId}`}>{value}</a>

                return (<a href={`https://fb.com/${row.FbId}`}>{value}</a>);
            }
        },
        {
            key: 2,
            title: "Số điện thoại",
            dataIndex: "Phones",
            width: 200,
            render: (value, parent) => {
                if (parent && parent.children) {
                    if (value && value.length) {
                        return (
                            <Space
                                direction="vertical"
                            >
                                {
                                    value.map((phone) => {
                                        return (<Tag color="green">{phone}</Tag>);
                                    })
                                }
                            </Space>
                        )
                    }
                    else {
                        return <Tag></Tag>;
                    }
                }
                else {
                    return (<Tag>{value ? value.toString() : ""}</Tag>);
                }
            },
            align: "center"
        },
        {
            key: 3,
            title: "Bình luận",
            dataIndex: "Comment",
            render: (value, parent) => {
                if (parent.children) {
                    return <Tag></Tag>
                }
                else {
                    return value;
                }
            }
        },
        {
            key: 4,
            title: "Thời gian",
            width: 100,
            dataIndex: "CreatedTime",
            render: (value, parent) => {
                if (parent.children) {
                    return <Tag></Tag>
                }
                else {
                    return moment(value).format("hh:mm:ss");
                }
            },
            align: "center"
        },
        {
            key: 5,
            title: "Hành động",
            width: 150,
            render: (value) => {
                if (value.children) {
                    return (
                        <Button type="primary"
                            style={{ backgroundColor: "#D46B08", borderColor: "#D46B08" }}
                            onClick={() => {
                                setCommentLoading(true);
                                HttpPutBlockUser(pageId, value.FbId)
                                    .then(() => { toast.success("Thêm vào sổ đen thành công."); setCommentLoading(false); })
                                    .catch(() => { toast.error("Thêm vào sổ đen thất bại."); setCommentLoading(false); });
                            }}
                        >Thêm Vào Sổ Đen</Button>
                    );
                }
                else {
                    if (value.IsSendWhenDetectedPhoneResult || value.IsSendWhenDetectedGoodWordResult) {
                        return (
                            <Space>
                                {/* <Button type="default" onClick={() => HttpPostHiddenComment(value.CommentId, value.FbId, value.videoId, value.IsHiddenResult ? false : true)}>{value.IsHiddenResult ? "Hiện" : "Ẩn"}</Button> */}
                                <Tag color="blue">ĐÃ GỬI</Tag>
                            </Space>
                        );
                    }
                    else if (value.IsHiddenResult) {
                        return (
                            <Space>
                                <Tag color="yellow">{value.IsHiddenResult ? "ĐÃ ẨN" : "HIỆN"}</Tag>
                            </Space>
                        );
                    }
                    else if (value.IsDeleteWhenDetectedBadWordResult) {
                        return (
                            <Space>
                                <Tag color="red">ĐÃ XÓA</Tag>
                            </Space>
                        );
                    }
                    else {
                        return (
                            <Space>
                                {/* <Button type="default" onClick={() => HttpPostHiddenComment(value.CommentId, value.FbId, value.videoId, value.IsHiddenResult ? false : true)}>{value.IsHiddenResult ? "Hiện" : "Ẩn"}</Button> */}
                                <Button type="primary" danger onClick={() => HttpDeleteComment(value.CommentId, value.FbId, value.videoId)} >Xóa</Button>
                                <Button type="primary" onClick={() => HttpPostSendPrivateReply(value.CommentId, value.FbId, value.videoId, localStorage.getItem("pageId"))}>Gửi</Button>
                            </Space>
                        )
                    }
                }
            },
            align: "center"
        }
    ]

    //#region Http Request
    const HttpGetVideo = (pageId) => {
        return new Promise((resolve, reject) => {
            if (pageId) {
                fetch(`${process.env.REACT_APP_API_HOST}/video?pageId=${pageId}`, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'access_token': token
                    },
                }).then(response => {
                    if (response.status !== 200) {
                        return reject(response);
                    }
                    return response.json();
                }).then(data => {
                    resolve(data);
                }).catch((err) => {
                    reject("ERROR: " + err)
                });
            }
        });
    }

    const HttpPostConfigVideo = ({ pageId, videoId, isSendWhenDetectedPhone, isSendWhenDetectedGoodWord, isDeleteWhenDetectedBadWord, isHiddenWhenNegSentiment, isHidden, goodWords, badWords, replyMessage }) => {
        console.log({ pageId, videoId, isSendWhenDetectedPhone, isSendWhenDetectedGoodWord, isDeleteWhenDetectedBadWord, isHiddenWhenNegSentiment, isHidden, goodWords, badWords, replyMessage })
        return new Promise((resolve, reject) => {
            if (pageId && videoId) {
                fetch(`${process.env.REACT_APP_API_HOST}/video/config`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'access_token': token
                    },
                    method: "POST",
                    body: JSON.stringify({
                        pageId,
                        videoId,
                        isSendWhenDetectedPhone,
                        isSendWhenDetectedGoodWord,
                        isDeleteWhenDetectedBadWord,
                        isHiddenWhenNegSentiment,
                        isHidden,
                        goodWords,
                        badWords,
                        replyMessage
                    })
                }).then((res) => {
                    if (res.status !== 200) {
                        return reject(res);
                    }
                    return res.json();
                }).then((data) => {
                    resolve(data);
                }).catch((err) => {
                    reject('ERROR: ' + err);
                });
            }
        });
    }

    const HttpGetComment = (liveId) => {
        const eSource = new EventSource(`${process.env.REACT_APP_API_HOST}/comment?id=${liveId}&access_token=${token}`);
        const info = {
            id: liveId,
            instance: eSource
        }
        setEventSources([...eventSources, info]);

        eSource.onmessage = (e) => {
            var res = JSON.parse(e.data);
            if (res.StatusCode === 200) {
                const resData = JSON.parse(res.Data);
                const data = resData.map((item, index) => {
                    return {
                        key: index,
                        videoId: liveId,
                        FbId: item.FbId,
                        FbName: item.FbName,
                        Phones: item.Phones,
                        CommentId: item.CommentId,
                        Comment: item.Comments[0].Message,
                        CreatedTime: item.Comments[0].CreatedTime,
                        children: item.Comments.map((cmt, index) => {
                            return {
                                key: index,
                                videoId: liveId,
                                FbId: item.FbId,
                                FbName: item.FbName,
                                // Phone: item.Phone,
                                CommentId: cmt.CommentId,
                                Comment: cmt.Message,
                                CreatedTime: cmt.CreatedTime,
                                IsDeleteWhenDetectedBadWordResult: cmt.IsDeleteWhenDetectedBadWordResult,
                                IsSendWhenDetectedGoodWordResult: cmt.IsSendWhenDetectedGoodWordResult,
                                IsSendWhenDetectedPhoneResult: cmt.IsSendWhenDetectedPhoneResult,
                                IsHiddenResult: cmt.IsHiddenResult,
                            }
                        })
                    }
                })

                // Remove comment
                let lstComment = comments.filter(cmt => cmt.id !== String(liveId));
                var newComments = lstComment;
                var comment = {
                    id: String(liveId),
                    data: data
                }
                newComments.push(comment);
                setComments(newComments);
            }
        }

        eSource.onerror = (e) => {
            eSource.close();
        }
    }

    const HttpPostSendPrivateReply = (commentId, fbId, videoId, pageId = localStorage.getItem("pageId")) => {
        setRootLoading(true);
        fetch(`${process.env.REACT_APP_API_HOST}/comment/send?commentId=${commentId}&fbId=${fbId}&videoId=${videoId}&pageId=${pageId}`, {
            // fetch(`https://localhost:44302/api/comment/send?commentId=${commentId}&fbId=${fbId}&videoId=${videoId}&pageId=${pageId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'access_token': token
            }
        }).then(response => {
            if (response.status === 200) {
                toast.success("Gửi tin nhắn thành công.");
            }
            else {
                toast.error("Gửi tin nhắn thất bại.");
            }
            setRootLoading(false);
        }).catch(er => {
            toast.error("Có lỗi trong quá trình gửi.");
            setRootLoading(false);
        });
    }

    const HttpDeleteComment = (commentId, fbId, videoId) => {
        setRootLoading(true);
        fetch(`${process.env.REACT_APP_API_HOST}/comment/delete?videoId=${videoId}&fbId=${fbId}&commentId=${commentId}`, {
            // fetch(`https://localhost:44302/api/comment/delete?videoId=${videoId}&fbId=${fbId}&commentId=${commentId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'access_token': token
            },
        }).then(response => {
            if (response.status === 200) {
                return response.json();
            }
        }).then(data => {
            if (data && data.status && data.status === 1) {
                toast.success("Xóa bình luận thành công.");
            }
            else {
                toast.error("Xóa bình luận thất bại.");
            }
            setRootLoading(false);
        }).catch(err => {
            toast.error("Có lỗi trong quá trình xóa.");
            setRootLoading(false);
        })
    }

    const HttpPostHiddenComment = (commentId, fbId, videoId, isHidden) => {
        console.log(commentId, videoId, fbId, isHidden)
        setRootLoading(true);
        fetch(`${process.env.REACT_APP_API_HOST}/comment/hidden?videoId=${videoId}&fbId=${fbId}&commentId=${commentId}&isHidden=${isHidden}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'access_token': token
            },
        }).then(response => {
            if (response.status === 200) {
                return response.json();
            }
        }).then(data => {
            if (data && data.status && data.status === 1) {
                toast.success(`${isHidden ? 'Ẩn' : 'Hiện'} bình luận thành công.`);
            }
            else {
                toast.error(`${isHidden ? 'Ẩn' : 'Hiện'} bình luận thành công.`);
            }
            setRootLoading(false);
        }).catch(err => {
            toast.error(`Có lỗi trong quá trình ${isHidden ? 'Ẩn' : 'Hiện'} bình luận.`);
            setRootLoading(false);
        })
    }

    const HttpPutBlockUser = (pageId, blockId) => {
        return new Promise((resolve, reject) => {
            if (pageId) {
                fetch(`${process.env.REACT_APP_API_HOST}/page/block?pageId=${pageId}&blockId=${blockId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'access_token': token
                    },
                }).then(response => {
                    if (response.status !== 200) {
                        return reject(response);
                    }
                    return response.json();
                }).then(data => {
                    resolve(data);
                }).catch((err) => {
                    reject("ERROR: " + err)
                });
            }
        });
    }

    const HttpGetExportExcel = (videoId) => {
        return new Promise((resolve, reject) => {
            if (pageId) {
                fetch(`${process.env.REACT_APP_API_HOST}/comment/export?Id=${videoId}`, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'access_token': token
                    },
                }).then(response => {
                    return response.blob();
                }).then(data => {
                    let url = window.URL.createObjectURL(data);
                    let a = document.createElement('a');
                    a.href = url;
                    a.download = `video_${videoId}.xlsx`;
                    a.click();
                    setRootLoading(false);
                }).catch((err) => {
                    reject("ERROR: " + err)
                });
            }
        });
    }
    //#endregion

    //#region pane
    const handleAddPane = (liveId, title) => {

        setAddPaneLoading(true);

        for (var i = 0; i < panes.length; i++) {
            if (panes[i]["key"] === String(liveId)) {
                return;
            }
        }

        HttpGetComment(String(liveId));

        const comment = comments && comments.length && comments.length > 0 ? comments.filter(cmt => cmt.id === String(liveId)) :
            '';

        if (comment !== undefined) {
            title = title === undefined || title === null ? `Bình luận` : `Bình luận - ${title}`;
            setPanes([
                ...panes,
                {
                    key: String(liveId),
                    tab: title,
                    columns: columns,
                }
            ])
        }
    }

    const handleChangePane = (key) => {
        setActivePane(String(key));
    }

    const handleEditPane = (targetKey, action) => {
        if (action === "remove") {
            setActivePane("0");
            disposePane(targetKey);
            disposeComment(targetKey);
            disposeEventSource(targetKey)
        }
    };
    //#endregion

    //#region Modal
    const showModal = ({ title, pageId, videoId, isSendWhenDetectedPhone, isSendWhenDetectedGoodWord, isDeleteWhenDetectedBadWord, isHiddenWhenNegSentiment, isHidden, goodWords, badWords, status, replyMessage }) => {
        setModalVisiable(true);
        setModelData({
            title,
            pageId,
            videoId,
            isSendWhenDetectedPhone,
            isSendWhenDetectedGoodWord,
            isDeleteWhenDetectedBadWord,
            isHiddenWhenNegSentiment,
            isHidden,
            goodWords,
            badWords,
            status,
            replyMessage
        });
    }

    const handleOk = () => {
        setModalVisiable(false);

        const body = {
            pageId: modelData.pageId,
            videoId: modelData.videoId,
            isSendWhenDetectedPhone: modelData.isSendWhenDetectedPhone,
            isSendWhenDetectedGoodWord: modelData.isSendWhenDetectedGoodWord,
            isDeleteWhenDetectedBadWord: modelData.isDeleteWhenDetectedBadWord,
            isHiddenWhenNegSentiment: modelData.isHiddenWhenNegSentiment,
            isHidden: modelData.isHidden,
            goodWords: modelData.goodWords,
            badWords: modelData.badWords,
            replyMessage: modelData.replyMessage
        }

        HttpPostConfigVideo(body).then(data => {
            toast.success("Cập nhật thành công.");
            HttpGetVideo(pageId).then(data => setVideos(data))
        }).catch(err => console.log(err));
    }

    const handleCancel = () => {
        setModalVisiable(false);
    }
    //#endregion

    return (
        <React.Fragment>
            <Spin spinning={rootLoading} >
                <Layout style={{ height: "100%" }} >
                    <Row>
                        <Col>
                            <Breadcrumb className="App-content-header">
                                <Breadcrumb.Item>Livestream</Breadcrumb.Item>
                            </Breadcrumb>
                        </Col>
                    </Row>
                    <Tabs
                        hideAdd
                        activeKey={activePane}
                        type="editable-card"
                        onChange={handleChangePane}
                        onEdit={handleEditPane}
                    >
                        <Tabs.TabPane tab="Live Video"
                            key="0"
                            closeIcon
                        >
                            <Row justify="center">
                                <Col xs={24} lg={24}>
                                    <Card>
                                        <Spin spinning={addPaneLoading}>
                                            <List
                                                loading={videoLoading}
                                                itemLayout="horizontal"
                                                dataSource={videos}
                                                renderItem={video =>
                                                    <List.Item
                                                        actions={
                                                            [
                                                                <Button key="list-loadmore-edit"
                                                                    danger
                                                                    type="primary"
                                                                    onClick={() => {
                                                                        const model = {
                                                                            title: video.title,
                                                                            pageId: pageId,
                                                                            videoId: video.videoId,
                                                                            isSendWhenDetectedPhone: video.isSendWhenDetectedPhone,
                                                                            isSendWhenDetectedGoodWord: video.isSendWhenDetectedGoodWord,
                                                                            isDeleteWhenDetectedBadWord: video.isDeleteWhenDetectedBadWord,
                                                                            isDeleteWhenDetectedBadWord: video.isDeleteWhenDetectedBadWord,
                                                                            isHiddenWhenNegSentiment: video.isHiddenWhenNegSentiment,
                                                                            isHidden: video.isHidden,
                                                                            goodWords: video.goodWords,
                                                                            badWords: video.badWords,
                                                                            status: video.status,
                                                                            replyMessage: video?.replyTemplate?.message?.attachment?.payload.text
                                                                        }
                                                                        showModal(model);
                                                                    }}
                                                                >Cấu hình</Button>,
                                                                <Button key="list-loadmore-more" type="primary" onClick={() => {
                                                                    handleAddPane(video.videoId, video.title);
                                                                    setTimeout(() => {
                                                                        setActivePane(video.videoId);
                                                                        setAddPaneLoading(false);
                                                                    }, 1000);
                                                                }}>Xem bình luận</Button>]
                                                        }
                                                    >
                                                        <List.Item.Meta
                                                            title={<Badge count={`${video.status}`}
                                                                offset={[30, 0]}
                                                                style={video.status === "LIVE" ? ({ background: "red" }) : ({ background: "#52c41a" })}
                                                            >
                                                                <a href={`https:fb.com${video.permalinkUrl}`}>{video.title}</a>
                                                            </Badge>}
                                                            description={video.description}
                                                        />
                                                    </List.Item>
                                                }
                                            />
                                        </Spin>
                                    </Card>
                                </Col>
                            </Row>
                        </Tabs.TabPane>

                        {
                            panes.map(pane => {
                                const comment = comments.find(cmt => cmt.id === pane.key);
                                return comment === undefined ? "" : (
                                    <CustomTabPane key={pane.key} tab={pane.tab} columns={pane.columns} loading={commnetLoading} dataSource={comment.data} videoId={pane.key} />
                                )
                            })
                        }

                    </Tabs>
                </Layout>
            </Spin>

            <Modal
                title={`${modelData.videoId} ${modelData.title || ""}`}
                visible={modalVisiable}
                onOk={handleOk}
                onCancel={handleCancel}
                okButtonProps={{ disabled: modelData.status === "LIVE" ? false : true }}
            >
                <Row>
                    <Space direction="vertical" style={{ width: '100%' }}>
                        <Collapse>
                            <Collapse.Panel header="Gửi tin nhắn riêng" key="1">
                                <Row>
                                    <Col span={24}>
                                        <Space style={{ width: "100%", justifyContent: "space-between" }}>
                                            <Typography.Text style={{ color: "blue" }}>Tự động gửi tin nhắn khi phát hiện SĐT</Typography.Text>
                                            <Switch checked={modelData.isSendWhenDetectedPhone} onChange={(checked) => setModelData({ ...modelData, isSendWhenDetectedPhone: checked })}
                                                disabled={modelData.status === "LIVE" ? false : true}
                                            />
                                        </Space>
                                    </Col>
                                </Row>
                                <Divider plain />
                                <Row>
                                    <Space direction="vertical" style={{ width: "100%" }}>
                                        <Col span={24}>
                                            <Space style={{ width: "100%", justifyContent: "space-between" }}>
                                                <Typography.Text style={{ color: "blue" }}>Tự động gửi tin nhắn khi phát hiện từ khóa</Typography.Text>
                                                <Switch checked={modelData.isSendWhenDetectedGoodWord} onChange={(checked) => setModelData({ ...modelData, isSendWhenDetectedGoodWord: checked })}
                                                    disabled={modelData.status === "LIVE" ? false : true}
                                                />
                                            </Space>
                                        </Col>
                                        <Col span={24}>
                                            <Input.TextArea maxLength={500}
                                                rows={4}
                                                disabled={modelData.status === "LIVE" ? false : true}
                                                value={modelData.goodWords?.toString()}
                                                onChange={({ target: { value } }) => {
                                                    setModelData({ ...modelData, goodWords: value.split(",").map((item) => item.trim()) });
                                                }}
                                            />
                                        </Col>
                                    </Space>
                                </Row>
                            </Collapse.Panel>
                        </Collapse>

                        <Collapse>
                            <Collapse.Panel header="Tin nhắn mẫu" key="1">
                                <Row>
                                    <Col span={24}>
                                        <Input.TextArea maxLength={500}
                                            // disabled={modelData.status === "LIVE" ? false : true}
                                            value={modelData.replyMessage}
                                            onChange={({ target: { value } }) => {
                                                setModelData({
                                                    ...modelData,
                                                    replyMessage: value
                                                })
                                            }}
                                        />
                                    </Col>
                                </Row>
                            </Collapse.Panel>
                        </Collapse>

                        <Collapse>
                            <Collapse.Panel header="Ẩn bình luận" key="1">
                                <Row>
                                    <Space direction="vertical" style={{ width: "100%" }}>
                                        <Col span={24}>
                                            <Space style={{ width: "100%", justifyContent: "space-between" }}>
                                                <Typography.Text style={{ color: "yellowgreen" }}>Tự động ẩn bình luận khi phát hiện SĐT</Typography.Text>
                                                <Switch checked={modelData.isHidden} onChange={(checked) => setModelData({ ...modelData, isHidden: checked })}
                                                    disabled={modelData.status === "LIVE" ? false : true}
                                                />
                                            </Space>
                                        </Col>
                                        <Divider />
                                        <Col span={24}>
                                            <Space style={{ width: "100%", justifyContent: "space-between" }}>
                                                <Typography.Text style={{ color: "yellowgreen" }}>Tự động ẩn bình luận khi phát hiện ý nghĩa tiêu cực</Typography.Text>
                                                <Switch checked={modelData.isHiddenWhenNegSentiment}
                                                    onChange={(checked) =>
                                                        setModelData({ ...modelData, isHiddenWhenNegSentiment: checked })
                                                    }
                                                    disabled={modelData.status === "LIVE" ? false : true}
                                                />
                                            </Space>
                                        </Col>
                                    </Space>
                                </Row>
                            </Collapse.Panel>
                        </Collapse>

                        <Collapse>
                            <Collapse.Panel header="Xóa bình luận" key="1">
                                <Row>
                                    <Space direction="vertical" style={{ width: "100%" }}>
                                        <Col span={24}>
                                            <Space style={{ width: "100%", justifyContent: "space-between" }}>
                                                <Typography.Text style={{ color: "red" }}>Tự động xóa bình luận khi phát hiện từ khóa</Typography.Text>
                                                <Switch checked={modelData.isDeleteWhenDetectedBadWord} onChange={(checked) => setModelData({ ...modelData, isDeleteWhenDetectedBadWord: checked })}
                                                    disabled={modelData.status === "LIVE" ? false : true}
                                                />
                                            </Space>
                                        </Col>
                                        <Col span={24}>
                                            <Input.TextArea maxLength={500}
                                                rows={4}
                                                disabled={modelData.status === "LIVE" ? false : true}
                                                value={modelData.badWords?.toString()}
                                                onChange={({ target: { value } }) => {
                                                    setModelData({ ...modelData, badWords: value.split(",").map((item) => item.trim()) });
                                                }}
                                            />
                                        </Col>
                                    </Space>
                                </Row>
                            </Collapse.Panel>
                        </Collapse>
                    </Space>
                </Row>
            </Modal>

            <ToastContainer />
        </React.Fragment>
    );
};

export default Livestream;